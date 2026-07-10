using Aquality.Selenium.Browsers;
using UnionReportingApi.TestData;

namespace UnionReportingApi.Utils;

public static class ConditionalWaitHelper
{
    public static void WaitForTrue(
        Func<bool> condition,
        string message,
        int? timeoutSeconds = null)
    {
        var testData = TestDataProvider.Instance.Data;
        AqualityServices.ConditionalWait.WaitForTrue(
            condition,
            TimeSpan.FromSeconds(timeoutSeconds ?? testData.ConditionTimeoutSeconds),
            TimeSpan.FromSeconds(testData.PollingIntervalSeconds),
            message);
    }
}
