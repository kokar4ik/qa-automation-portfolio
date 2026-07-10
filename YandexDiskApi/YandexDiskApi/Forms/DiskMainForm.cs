using Aquality.Selenium.Browsers;
using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using YandexDiskApi.Constants;
using YandexDiskApi.Helpers;
using YandexDiskApi.Utils;

namespace YandexDiskApi.Forms;

public class DiskMainForm : Form
{
    private static readonly By SignInButtonLocator = By.Id("header-login-button");
    private static readonly By UploadButtonLocator = By.XPath("//button[contains(., 'Загрузить')]");
    private static readonly By DeleteDialogLocator = By.CssSelector(".dialog__wrap");
    private static readonly By DeleteConfirmButtonLocator =
        By.XPath("//div[contains(@class,'dialog__wrap')]//*[text()='Удалить']");
    private static readonly By TrashLocator =
        By.CssSelector("span.LeftColumnNavigation__Item_type_trash");
    private const string FileListingItemXPathTemplate =
        "//div[contains(@class,'listing-item')]" +
        "[.//div[contains(@class,'listing-item__title')][@aria-label='{0}']]";

    public DiskMainForm()
        : base(SignInButtonLocator, "Главная страница Яндекс.Диска")
    {
    }

    private IButton SignInButton =>
        ElementFactory.GetButton(SignInButtonLocator, "Войти");

    private IButton UploadButton =>
        ElementFactory.GetButton(UploadButtonLocator, "Загрузить");

    private ILabel DeleteDialog =>
        ElementFactory.GetLabel(DeleteDialogLocator, "Диалог удаления");

    private IButton DeleteConfirmButton =>
        ElementFactory.GetButton(DeleteConfirmButtonLocator, "Подтверждение удаления");

    public bool IsUploadButtonDisplayed() =>
        UploadButton.State.IsDisplayed;

    public void ClickSignIn()
    {
        SignInButton.Click();
        AqualityServices.Browser.WaitForPageToLoad();
    }

    public bool IsPassportPageOpened() =>
        AqualityServices.Browser.CurrentUrl.Contains(UrlFragments.PassportHost, StringComparison.OrdinalIgnoreCase);

    public bool IsFileDisplayed(string fileName) =>
        GetFileItem(fileName).State.IsDisplayed;

    public bool IsTrashDisplayed()
    {
        var trash = FindTrashElement();
        return trash != null;
    }

    public void ScrollTrashIntoView()
    {
        var trash = FindTrashElement();
        if (trash != null)
        {
            JsExecutor.ScrollTrashIntoView(trash);
        }
    }

    public void RefreshPage()
    {
        AqualityServices.Browser.Refresh();
        AqualityServices.Browser.WaitForPageToLoad();
    }

    public void DoubleClickFile(string fileName)
    {
        var fileItem = GetFileItem(fileName);
        fileItem.JsActions.ScrollToTheCenter();
        BrowserTabsHelper.RememberMainTab();
        fileItem.MouseActions.DoubleClick();
    }

    public void SwitchToOpenedTab()
    {
        BrowserTabsHelper.SwitchToLastTab();
        AqualityServices.Browser.WaitForPageToLoad();
    }

    public void CloseCurrentTabAndReturnToMain()
    {
        BrowserTabsHelper.CloseCurrentTabAndReturnToMain();
    }

    public void DragFileToTrash(string fileName)
    {
        var driver = AqualityServices.Browser.Driver;
        var source = AqualityServices.Browser.Driver.FindElements(
            By.XPath(string.Format(FileListingItemXPathTemplate, fileName)))[0];
        var target = FindTrashElement()!;

        JsExecutor.ScrollIntoView(source);
        JsExecutor.ScrollTrashIntoView(target);
        new Actions(driver).DragAndDrop(source, target).Perform();
    }

    public bool IsDeleteDialogDisplayed() =>
        DeleteDialog.State.IsDisplayed;

    public void ClickDeleteConfirm() =>
        DeleteConfirmButton.Click();

    private ILabel GetFileItem(string fileName) =>
        ElementFactory.GetLabel(
            By.XPath(string.Format(FileListingItemXPathTemplate, fileName)),
            $"Файл {fileName.ToQuote()}");

    private static IWebElement? FindTrashElement()
    {
        var elements = AqualityServices.Browser.Driver.FindElements(TrashLocator);
        return elements.Count == 0 ? null : elements[0];
    }
}
