using System.Net;
using System.Net.Mime;
using System.Text;
using Aquality.Selenium.Browsers;
using YandexDiskApi.Api.Models;
using YandexDiskApi.Constants;
using YandexDiskApi.Forms;
using YandexDiskApi.Helpers;
using YandexDiskApi.Steps;
using YandexDiskApi.TestData;
using YandexDiskApi.Utils;

namespace YandexDiskApi.Tests;

public class YandexDiskE2ETest : TestBase
{
    [Test]
    [Explicit("Требуется ручное подтверждение 2FA.")]
    public void YandexDiskFullScenario()
    {
        var testData = TestDataProvider.Instance.Data;
        var timeouts = TestDataProvider.Instance.Timeouts;

        var textFileName = TestDataGenerator.CreateFileName(testData.TextFileExtension);
        var imageFileName = Path.ChangeExtension(textFileName, $".{testData.ImageFileExtension}");
        var fileContent = TestDataGenerator.CreateRandomText();
        var textFilePath = DiskPathUtils.Normalize(textFileName);
        var imageFilePath = DiskPathUtils.Normalize(imageFileName);
        var imageResourcePath = TestDataProvider.ResolveResourcePath(testData.ImageResourcePath);
        var expectedImageBytes = File.ReadAllBytes(imageResourcePath);
        var expectedTextBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileContent));

        var diskMainForm = new DiskMainForm();
        var loginForm = new LoginForm();
        var loginSteps = new LoginSteps();

        AqualityServices.Logger.Info("Шаг 1. Авторизация в Яндекс.Диске.");
        if (!diskMainForm.IsUploadButtonDisplayed())
        {
            AqualityServices.Logger.Info("Пользователь не авторизован. Открытие формы входа.");
            diskMainForm.ClickSignIn();

            ConditionalWaitHelper.WaitForTrue(
                () => diskMainForm.IsPassportPageOpened(),
                "Страница входа Passport не открылась после нажатия «Войти».",
                timeouts.LoginSeconds);
            Assert.That(diskMainForm.IsPassportPageOpened(), Is.True, "Страница входа Passport не открылась.");

            ConditionalWaitHelper.WaitForTrue(
                () => loginForm.State.IsDisplayed,
                "Форма входа не отображается.",
                timeouts.LoginSeconds);
            Assert.That(loginForm.State.IsDisplayed, Is.True, "Форма входа не отображается.");

            loginSteps.LoginWithConfiguration();

            AqualityServices.Logger.Info(
                "Если запрошен код подтверждения (2FA), введите его вручную в браузере.");
            ConditionalWaitHelper.WaitForTrue(
                () => loginForm.RemindLaterButton.State.IsDisplayed || diskMainForm.IsUploadButtonDisplayed(),
                "Вход не завершён: не появились Яндекс.Диск или запрос отпечатка после авторизации.",
                timeouts.FingerprintSeconds);

            loginSteps.DismissFingerprintPromptIfDisplayed();

            ConditionalWaitHelper.WaitForTrue(
                () => diskMainForm.IsUploadButtonDisplayed(),
                "Яндекс.Диск не открылся после входа.",
                timeouts.ManualConfirmationSeconds);
        }

        Assert.That(diskMainForm.IsUploadButtonDisplayed(), Is.True, "Яндекс.Диск не открылся после входа.");
        AqualityServices.Logger.Info("Авторизация завершена.");

        AqualityServices.Logger.Info("Шаг 2. Проверка доступности API.");
        var diskResponse = ApiClient.GetDisk();
        AqualityServices.Logger.Info($"Ответ API /disk: {(int)diskResponse.StatusCode} {diskResponse.StatusCode}");
        Assert.That(diskResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        AqualityServices.Logger.Info($"Шаг 3. Загрузка текстового файла {textFileName.ToQuote()} через API.");
        var textUploadLinkResponse = ApiClient.GetUploadLink(textFilePath, overwrite: false);
        AqualityServices.Logger.Info(
            $"Ответ API upload link: {(int)textUploadLinkResponse.StatusCode} {textUploadLinkResponse.StatusCode}");
        Assert.That(textUploadLinkResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(textUploadLinkResponse.Data?.Href, Is.Not.Empty);

        var textBytes = Encoding.UTF8.GetBytes(fileContent);
        var textUploadResponse = ApiClient.UploadToHref(
            textUploadLinkResponse.Data!.Href,
            textBytes,
            ApiMediaTypes.TextPlainUtf8);
        AqualityServices.Logger.Info(
            $"Ответ загрузки текста: {(int)textUploadResponse.StatusCode} {textUploadResponse.StatusCode}");
        Assert.That(textUploadResponse.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Created));

        AqualityServices.Logger.Info("Шаг 4. Проверка отображения текстового файла в UI.");
        ConditionalWaitHelper.WaitForTrue(
            () => diskMainForm.IsFileDisplayed(textFileName),
            $"Файл {textFileName.ToQuote()} не отображается на диске.",
            timeouts.FileDisplayedSeconds);
        Assert.That(diskMainForm.IsFileDisplayed(textFileName), Is.True);

        AqualityServices.Logger.Info("Шаг 5. Открытие и проверка содержимого текстового файла.");
        diskMainForm.DoubleClickFile(textFileName);
        diskMainForm.SwitchToOpenedTab();

        var documentViewer = new DocumentViewerForm();
        ConditionalWaitHelper.WaitForTrue(
            () => documentViewer.IsTitleDisplayed(textFileName) || documentViewer.IsTextLoaded(),
            $"Просмотр документа {textFileName.ToQuote()} не открылся.",
            timeouts.DocumentOpenSeconds);
        Assert.That(
            documentViewer.IsTitleDisplayed(textFileName) || documentViewer.IsTextLoaded(),
            Is.True,
            $"Просмотр документа {textFileName.ToQuote()} не открылся.");

        ConditionalWaitHelper.WaitForTrue(
            () => documentViewer.IsTextLoaded(),
            "Содержимое страницы документа не загрузилось.",
            timeouts.DocumentTextSeconds);
        Assert.That(documentViewer.IsTextLoaded(), Is.True, "Содержимое страницы документа не загрузилось.");

        var rawDocumentText = documentViewer.GetRawText();
        var normalizedDocumentText = TextUtils.NormalizeViewerText(rawDocumentText);
        var cleanedDocumentText = TextUtils.StripWrappingQuotes(rawDocumentText.Trim());
        AqualityServices.Logger.Info($"Сырой текст документа: {rawDocumentText.ToQuote()}");
        AqualityServices.Logger.Info($"Нормализованный текст документа: {normalizedDocumentText.ToQuote()}");

        Assert.That(
            normalizedDocumentText.Contains(fileContent, StringComparison.Ordinal)
                || cleanedDocumentText.Contains(fileContent, StringComparison.Ordinal)
                || cleanedDocumentText.Contains(expectedTextBase64, StringComparison.Ordinal),
            Is.True,
            $"Текст документа не содержит ожидаемое содержимое {fileContent.ToQuote()}. " +
            $"Сырой: {rawDocumentText.ToQuote()}. Нормализованный: {normalizedDocumentText.ToQuote()}.");

        diskMainForm.CloseCurrentTabAndReturnToMain();
        Assert.That(diskMainForm.IsUploadButtonDisplayed(), Is.True, "Не удалось вернуться на страницу диска.");

        AqualityServices.Logger.Info($"Шаг 6. Переименование файла в {imageFileName.ToQuote()} через API.");
        var moveResponse = ApiClient.MoveResource(textFilePath, imageFilePath);
        AqualityServices.Logger.Info($"Ответ API move: {(int)moveResponse.StatusCode} {moveResponse.StatusCode}");
        Assert.That(moveResponse.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Created));

        ConditionalWaitHelper.WaitForTrue(
            () => diskMainForm.IsFileDisplayed(imageFileName),
            $"Файл {imageFileName.ToQuote()} не отображается на диске.",
            timeouts.FileDisplayedSeconds);
        Assert.That(diskMainForm.IsFileDisplayed(imageFileName), Is.True);
        Assert.That(diskMainForm.IsFileDisplayed(textFileName), Is.False);

        AqualityServices.Logger.Info("Шаг 7. Загрузка изображения через API.");
        var imageUploadLinkResponse = ApiClient.GetUploadLink(imageFilePath, overwrite: true);
        Assert.That(imageUploadLinkResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(imageUploadLinkResponse.Data?.Href, Is.Not.Empty);

        var imageUploadResponse = ApiClient.UploadToHref(
            imageUploadLinkResponse.Data!.Href,
            expectedImageBytes,
            MediaTypeNames.Image.Jpeg);
        AqualityServices.Logger.Info(
            $"Ответ загрузки изображения: {(int)imageUploadResponse.StatusCode} {imageUploadResponse.StatusCode}");
        Assert.That(imageUploadResponse.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.Created));

        ConditionalWaitHelper.WaitForTrue(
            () => diskMainForm.IsFileDisplayed(imageFileName),
            $"Файл {imageFileName.ToQuote()} не отображается на диске.",
            timeouts.FileDisplayedSeconds);
        Assert.That(diskMainForm.IsFileDisplayed(imageFileName), Is.True);

        AqualityServices.Logger.Info("Шаг 8. Проверка содержимого изображения через API.");
        var downloadLinkResponse = ApiClient.GetDownloadLink(imageFilePath);
        AqualityServices.Logger.Info(
            $"Ответ API download link: {(int)downloadLinkResponse.StatusCode} {downloadLinkResponse.StatusCode}");
        Assert.That(downloadLinkResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(downloadLinkResponse.Data?.Href, Is.Not.Empty);

        var downloadedImageResponse = ApiClient.DownloadFromHref(downloadLinkResponse.Data!.Href);
        AqualityServices.Logger.Info(
            $"Ответ скачивания изображения: {(int)downloadedImageResponse.StatusCode} {downloadedImageResponse.StatusCode}");
        Assert.That(downloadedImageResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(downloadedImageResponse.RawBytes, Is.EqualTo(expectedImageBytes));

        AqualityServices.Logger.Info("Шаг 9. Открытие превью изображения в UI.");
        diskMainForm.DoubleClickFile(imageFileName);
        diskMainForm.SwitchToOpenedTab();

        var imageViewer = new ImageViewerForm();
        ConditionalWaitHelper.WaitForTrue(
            () => imageViewer.IsPreviewDisplayed(),
            "Превью изображения не отображается.",
            timeouts.ImageDownloadSeconds);
        Assert.That(imageViewer.IsPreviewDisplayed(), Is.True);

        var imageSource = imageViewer.GetImageSource();
        AqualityServices.Logger.Info($"Адрес превью изображения: {imageSource.ToQuote()}");
        Assert.That(imageSource, Is.Not.Empty);
        Assert.That(
            imageSource,
            Does.Contain(UrlFragments.ImageDownloaderHost),
            "Превью изображения не содержит адрес загрузчика Яндекс.Диска.");

        imageViewer.Close();
        diskMainForm.CloseCurrentTabAndReturnToMain();

        ConditionalWaitHelper.WaitForTrue(
            () => diskMainForm.IsFileDisplayed(imageFileName),
            $"Файл {imageFileName.ToQuote()} не отображается на диске.",
            timeouts.FileDisplayedSeconds);
        Assert.That(diskMainForm.IsFileDisplayed(imageFileName), Is.True);

        AqualityServices.Logger.Info("Шаг 10. Удаление файла в корзину через UI.");
        ConditionalWaitHelper.WaitForTrue(
            () => diskMainForm.IsTrashDisplayed(),
            "Ссылка «Корзина» не найдена на странице.",
            timeouts.ConditionSeconds);
        Assert.That(diskMainForm.IsTrashDisplayed(), Is.True);
        Assert.That(diskMainForm.IsFileDisplayed(imageFileName), Is.True);
        diskMainForm.ScrollTrashIntoView();
        diskMainForm.DragFileToTrash(imageFileName);

        ConditionalWaitHelper.WaitForTrue(
            () => diskMainForm.IsDeleteDialogDisplayed(),
            "Диалог подтверждения удаления не отображается.",
            timeouts.FileDisplayedSeconds);
        Assert.That(diskMainForm.IsDeleteDialogDisplayed(), Is.True);
        diskMainForm.ClickDeleteConfirm();

        ConditionalWaitHelper.WaitForTrue(
            () => !diskMainForm.IsFileDisplayed(imageFileName),
            $"Файл {imageFileName.ToQuote()} всё ещё отображается на диске.",
            timeouts.FileDisappearedSeconds);
        Assert.That(diskMainForm.IsFileDisplayed(imageFileName), Is.False);

        AqualityServices.Logger.Info("Шаг 11. Проверка наличия файла в корзине через API.");
        TrashItem? trashItem = null;
        ConditionalWaitHelper.WaitForTrue(
            () =>
            {
                var trashResponse = ApiClient.GetTrashResources();
                if (trashResponse.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                trashItem = trashResponse.Data?.Embedded?.Items
                    .FirstOrDefault(item => string.Equals(item.Name, imageFileName, StringComparison.OrdinalIgnoreCase));
                return trashItem != null;
            },
            $"Файл {imageFileName.ToQuote()} не найден в корзине через API.",
            timeouts.TrashSeconds);

        var trashListResponse = ApiClient.GetTrashResources();
        Assert.That(trashListResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(trashItem, Is.Not.Null);
        Assert.That(trashItem!.Path, Is.Not.Empty);

        AqualityServices.Logger.Info("Шаг 12. Восстановление файла из корзины через API.");
        var restoreResponse = ApiClient.RestoreFromTrash(trashItem.Path);
        AqualityServices.Logger.Info($"Ответ API restore: {(int)restoreResponse.StatusCode} {restoreResponse.StatusCode}");
        Assert.That(restoreResponse.StatusCode, Is.AnyOf(HttpStatusCode.Created, HttpStatusCode.Accepted));

        ConditionalWaitHelper.WaitForTrue(
            () => diskMainForm.IsFileDisplayed(imageFileName),
            $"Файл {imageFileName.ToQuote()} не отображается на диске.",
            timeouts.FileDisplayedSeconds);
        Assert.That(diskMainForm.IsFileDisplayed(imageFileName), Is.True);

        AqualityServices.Logger.Info("Шаг 13. Безвозвратное удаление файла через API.");
        var deleteResponse = ApiClient.DeletePermanently(imageFilePath);
        AqualityServices.Logger.Info($"Ответ API delete: {(int)deleteResponse.StatusCode} {deleteResponse.StatusCode}");
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        AqualityServices.Logger.Info("Шаг 14. Проверка отсутствия файла в UI после обновления страницы.");
        diskMainForm.RefreshPage();
        ConditionalWaitHelper.WaitForTrue(
            () => !diskMainForm.IsFileDisplayed(imageFileName),
            $"Файл {imageFileName.ToQuote()} всё ещё отображается на диске.",
            timeouts.FileDisappearedSeconds);
        Assert.That(diskMainForm.IsFileDisplayed(imageFileName), Is.False);

        AqualityServices.Logger.Info("Сценарий YandexDiskFullScenario завершён успешно.");
    }
}