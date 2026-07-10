using Aquality.Selenium.Browsers;
using YandexDiskApi.Api;
using YandexDiskApi.Config;

namespace YandexDiskApi.Tests;

[TestFixture]
public abstract class TestBase
{
    protected YandexDiskApiClient ApiClient { get; private set; } = null!;

    [SetUp]
    public void SetUpBrowser()
    {
        AqualityServices.Browser.Maximize();
        AqualityServices.Browser.GoTo(TestConfiguration.Instance.BaseUrl);
        AqualityServices.Browser.WaitForPageToLoad();

        ApiClient = YandexDiskApiClient.FromConfiguration();
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