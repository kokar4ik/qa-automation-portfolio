using Aquality.Selenium.Browsers;
using OpenQA.Selenium;
using UnionReportingApi.Config;
using UnionReportingApi.Constants;
using UnionReportingApi.Forms;
using UnionReportingApi.TestData;

namespace UnionReportingApi.Steps;

public class AuthSteps
{
    public void SetTokenCookie(string token)
    {
        var cookie = new Cookie(
            WebConstants.Cookies.Token,
            token,
            WebConstants.Cookies.Path,
            DateTime.Now.AddDays(WebConstants.Cookies.LifetimeDays));

        AqualityServices.Browser.Driver.Manage().Cookies.AddCookie(cookie);
        AqualityServices.Logger.Info("Cookie token установлен.");
    }

    public void RefreshProjectsPage()
    {
        var projectsForm = new ProjectsForm();
        projectsForm.RefreshPage();
    }

    public string GetFooterVersionText()
    {
        var projectsForm = new ProjectsForm();
        return projectsForm.GetFooterText();
    }

    public string GetExpectedFooterVersionText()
    {
        var testData = TestDataProvider.Instance.Data;
        return $"{testData.ExpectedFooterVersionPrefix}{TestConfiguration.Instance.VariantId}";
    }
}