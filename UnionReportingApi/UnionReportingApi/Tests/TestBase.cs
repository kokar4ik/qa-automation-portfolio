using Aquality.Selenium.Browsers;
using UnionReportingApi.Api;

namespace UnionReportingApi.Tests;

[TestFixture]
public abstract class TestBase
{
    protected ReportingApiClient ApiClient { get; private set; } = null!;

    [SetUp]
    public void SetUpBrowser()
    {
        AqualityServices.Browser.Maximize();
        ApiClient = ReportingApiClient.FromConfiguration();
    }

    [TearDown]
    public void TearDownBrowser()
    {
        ApiClient?.Dispose();

        if (AqualityServices.IsBrowserStarted)
        {
            AqualityServices.Browser.Quit();
        }
    }
}
