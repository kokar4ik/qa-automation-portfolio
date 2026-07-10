using Aquality.Selenium.Browsers;
using Aquality.Selenium.Core.Configurations;

namespace UserInyerface.Tests
{
    public abstract class BaseTest
    {
        [SetUp]
        public void Setup()
        {
            var baseUrl = AqualityServices.Get<ISettingsFile>().GetValue<string>(".baseUrl");
            AqualityServices.Browser.GoTo(baseUrl);
            AqualityServices.Browser.WaitForPageToLoad();
        }

        [TearDown]
        public void TearDown()
        {
            if (AqualityServices.IsBrowserStarted)
            {
                AqualityServices.Browser.Quit();
            }
        }
    }
}
