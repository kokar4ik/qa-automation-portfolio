using System.Globalization;
using Aquality.Selenium.Browsers;
using OpenQA.Selenium;
using RestSharp;
using UnionReportingApi.Api;
using UnionReportingApi.Api.Models;
using UnionReportingApi.Config;
using UnionReportingApi.Constants;
using UnionReportingApi.Forms;
using UnionReportingApi.Steps;
using UnionReportingApi.TestData;
using UnionReportingApi.Utils;

namespace UnionReportingApi.Tests;

[Category("UnionReporting")]
[Category("Variant2")]
public class UnionReportingVariant2Scenario : TestBase
{
    [Test]
    [Category("E2E")]
    public void UnionReportingVariant2AllStepsShouldPass()
    {
        var testData = TestDataProvider.Instance.Data;
        var configuration = TestConfiguration.Instance;
        var loginSteps = new LoginSteps();
        var authSteps = new AuthSteps();
        var projectSteps = new ProjectSteps();
        var projectsForm = new ProjectsForm();
        var allTestsForm = new AllTestsForm();
        var testInfoForm = new TestInfoForm();

        AqualityServices.Logger.Info("Шаг 1: получение токена через API.");
        var tokenResponse = ApiClient.GenerateToken(configuration.VariantId);
        Assert.That(tokenResponse.IsSuccessful, Is.True, "Запрос токена должен быть успешным.");
        var token = tokenResponse.Content?.Trim() ?? string.Empty;
        Assert.That(token, Is.Not.Empty, "API должен вернуть токен в теле ответа.");

        AqualityServices.Logger.Info("Шаг 2: авторизация + cookie token + проверка футера Version: 2.");
        loginSteps.OpenProjectsPageWithBasicAuth();
        authSteps.SetTokenCookie(token);
        authSteps.RefreshProjectsPage();
        Assert.That(
            authSteps.GetFooterVersionText(),
            Does.Contain(authSteps.GetExpectedFooterVersionText()),
            "В футере должен отображаться номер варианта.");

        AqualityServices.Logger.Info("Шаг 3: сравнение UI Nexage (1 страница) с API JSON после сортировки по startTime desc.");
        projectsForm.OpenProject(testData.NexageProjectName);
        Assert.That(allTestsForm.State.IsDisplayed, Is.True, "Страница тестов Nexage должна открыться.");

        ConditionalWaitHelper.WaitForTrue(
            allTestsForm.IsTestsTableDisplayed,
            "Таблица тестов Nexage должна загрузиться (AJAX).",
            testData.AjaxRefreshTimeoutSeconds);

        var uiRows = allTestsForm.GetFirstPageTestRows();
        Assert.That(uiRows, Is.Not.Empty, "На странице Nexage должен быть хотя бы один тест.");

        var compareCount = Math.Min(uiRows.Count, testData.TestsPerPage);
        var uiSlice = uiRows.Take(compareCount).ToList();

        Assert.That(
            ApiClient.TryGetProjectTestsFromJson(testData.NexageProjectId, out var apiTests, out var apiTestsResponse),
            Is.True,
            GetApiFormatFailureMessage(ApiConstants.ResponseFormats.Json, testData.NexageProjectId, apiTestsResponse));

        Assert.Multiple(() =>
        {
            Assert.That(apiTestsResponse.IsSuccessful, Is.True, "Запрос списка тестов Nexage (JSON) должен быть успешным.");
            Assert.That(apiTests, Is.Not.Empty, "API должен вернуть список тестов для Nexage.");
        });

        var apiSlice = apiTests
            .OrderByDescending(item => ParseStartTime(item.StartTime))
            .Take(compareCount)
            .ToList();

        AssertTestListsMatch(uiSlice, apiSlice);

        AqualityServices.Logger.Info("Шаг 4: создание проекта через +Add (новая вкладка).");
        projectsForm.NavigateBack();
        var projectName = projectSteps.CreateProjectInNewTab();
        Assert.That(
            projectsForm.IsProjectDisplayed(projectName),
            Is.True,
            "Созданный проект должен отображаться в списке.");

        AqualityServices.Logger.Info("Шаг 5: создание теста через API (test+log+attachment) и ожидание появления в UI.");
        projectsForm.OpenProject(projectName);

        ConditionalWaitHelper.WaitForTrue(
            allTestsForm.IsTestsTableDisplayed,
            "Таблица тестов проекта должна загрузиться (AJAX).",
            testData.ConditionTimeoutSeconds);

        var screenshotBase64 = TakeScreenshotAsBase64();
        var testName = $"{testData.TestNamePrefix}{Guid.NewGuid():N}";
        var sessionId = Guid.NewGuid().ToString("N");
        AqualityServices.Logger.Info($"SID={sessionId}");

        var createTestRequest = new CreateTestRequest
        {
            SessionId = sessionId,
            ProjectName = projectName,
            TestName = testName,
            MethodName = testData.TestMethodName,
            Environment = testData.DefaultEnvironment,
            Browser = testData.DefaultBrowser,
        };

        var createTestResponse = ApiClient.CreateTest(createTestRequest);
        Assert.That(createTestResponse.IsSuccessful, Is.True, "Создание теста через API должно быть успешным.");
        Assert.That(
            ReportingApiClient.TryParseTestId(createTestResponse, out var testId),
            Is.True,
            $"API вернул некорректный testId: {createTestResponse.Content}");

        var logResponse = ApiClient.AddLog(testId, testData.DefaultLogContent);
        Assert.That(logResponse.IsSuccessful, Is.True, "Добавление лога через API должно быть успешным.");

        var attachmentResponse = ApiClient.AddAttachment(
            testId,
            new AttachmentContent
            {
                Content = screenshotBase64,
                ContentType = ApiConstants.Protocol.MediaTypeImagePng,
            });
        Assert.That(attachmentResponse.IsSuccessful, Is.True, "Добавление вложения через API должно быть успешным.");

        ConditionalWaitHelper.WaitForTrue(
            () => allTestsForm.IsTestDisplayed(testName),
            $"Тест {testName} должен появиться на странице без обновления.",
            testData.ConditionTimeoutSeconds);

        AqualityServices.Logger.Info("Шаг 6: открытие теста и проверка полей/лога/скриншота.");
        allTestsForm.OpenTest(testName);
        Assert.That(testInfoForm.State.IsDisplayed, Is.True, "Страница теста должна открыться.");

        var logLines = testInfoForm.GetLogLines();
        var attachmentSource = testInfoForm.GetAttachmentImageSource();

        Assert.Multiple(() =>
        {
            Assert.That(
                testInfoForm.GetCommonInfoValue(WebConstants.TestInfoFields.ProjectName),
                Is.EqualTo(projectName));
            Assert.That(
                testInfoForm.GetCommonInfoValue(WebConstants.TestInfoFields.TestName),
                Is.EqualTo(testName));
            Assert.That(
                testInfoForm.GetCommonInfoValue(WebConstants.TestInfoFields.TestMethodName),
                Is.EqualTo(testData.TestMethodName));
            Assert.That(
                testInfoForm.GetCommonInfoValue(WebConstants.TestInfoFields.Status),
                Is.EqualTo(testData.DefaultTestStatus));
            Assert.That(
                testInfoForm.GetCommonInfoValue(WebConstants.TestInfoFields.Environment),
                Is.EqualTo(testData.DefaultEnvironment));
            Assert.That(
                testInfoForm.GetCommonInfoValue(WebConstants.TestInfoFields.Browser),
                Is.EqualTo(testData.DefaultBrowser));
            Assert.That(logLines, Does.Contain(testData.DefaultLogContent));
            Assert.That(attachmentSource, Does.Contain(screenshotBase64));
        });
    }

    private static string GetApiFormatFailureMessage(string expectedFormat, long projectId, RestResponse response)
    {
        var maxLength = ApiConstants.Protocol.ErrorResponseSnippetMaxLength;
        var code = response.StatusCode != 0 ? $"{(int)response.StatusCode} {response.StatusCode}" : "no status";
        var snippet = JsonConvertHelper.GetContentSnippet(response.Content, maxLength);
        return $"API не вернул {expectedFormat}-формат для projectId={projectId}. status={code}. body[0..{maxLength}]={snippet}";
    }

    private static string TakeScreenshotAsBase64() =>
        ((ITakesScreenshot)AqualityServices.Browser.Driver)
            .GetScreenshot()
            .AsBase64EncodedString;

    private static void AssertTestListsMatch(
        IReadOnlyList<UiTestRow> uiRows,
        IReadOnlyList<ProjectTestItem> apiRows)
    {
        Assert.That(
            apiRows.Count,
            Is.EqualTo(uiRows.Count),
            "Число тестов в UI и в отсортированном фрагменте API должно совпадать.");

        for (var index = 0; index < uiRows.Count; index++)
        {
            var uiRow = uiRows[index];
            var apiRow = apiRows[index];

            Assert.That(uiRow.Name, Is.EqualTo(apiRow.Name),
                $"Имя теста в строке {index + 1} не совпадает.");
            Assert.That(uiRow.Method, Is.EqualTo(apiRow.Method),
                $"Метод в строке {index + 1} не совпадает.");
            Assert.That(
                NormalizeStatus(uiRow.Status),
                Is.EqualTo(NormalizeStatus(apiRow.Status)),
                $"Статус в строке {index + 1} не совпадает.");
            Assert.That(
                NormalizeStartTime(uiRow.StartTime),
                Is.EqualTo(NormalizeStartTime(apiRow.StartTime)),
                $"Время старта в строке {index + 1} не совпадает.");
        }
    }

    private static string NormalizeStatus(string? value)
    {
        var normalized = (value ?? string.Empty).Trim();
        return normalized.ToUpperInvariant();
    }

    private static DateTime ParseStartTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DateTime.MinValue;
        }

        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeLocal,
            out var parsed)
            ? parsed
            : DateTime.MinValue;
    }

    private static string NormalizeStartTime(string? value)
    {
        var parsed = ParseStartTime(value);
        return parsed == DateTime.MinValue
            ? value ?? string.Empty
            : parsed.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
}