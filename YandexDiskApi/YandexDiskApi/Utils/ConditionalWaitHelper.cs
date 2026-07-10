using Aquality.Selenium.Browsers;
using YandexDiskApi.TestData;

namespace YandexDiskApi.Utils;

public static class ConditionalWaitHelper
{
    public static void WaitForTrue(
        Func<bool> condition,
        string message,
        int? timeoutSeconds = null)
    {
        var timeouts = TestDataProvider.Instance.Timeouts;
        AqualityServices.ConditionalWait.WaitForTrue(
            condition,
            TimeSpan.FromSeconds(timeoutSeconds ?? timeouts.ConditionSeconds),
            TimeSpan.FromSeconds(timeouts.PollingSeconds),
            message);
    }
}
