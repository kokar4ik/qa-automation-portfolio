using Aquality.Selenium.Browsers;
using UnionReportingApi.Config;
using UnionReportingApi.Constants;
using UnionReportingApi.Helpers;

namespace UnionReportingApi.Steps;

public class LoginSteps
{
    public void OpenProjectsPageWithBasicAuth()
    {
        var configuration = TestConfiguration.Instance;
        var url = AuthenticatedUrlBuilder.Build(
            configuration.BaseUrl,
            WebConstants.Paths.ProjectsPage,
            configuration.Login,
            configuration.Password);

        AqualityServices.Logger.Info("Открытие страницы проектов с Basic Auth.");
        AqualityServices.Browser.GoTo(url);
        AqualityServices.Browser.WaitForPageToLoad();
    }
}