using Aquality.Selenium.Browsers;
using UnionReportingApi.Constants;

namespace UnionReportingApi.Helpers;

public static class BrowserTabsHelper
{
    private static string? mainWindowHandle;

    public static void RememberMainTab() =>
        mainWindowHandle = AqualityServices.Browser.Windows().CurrentHandle;

    public static void SwitchToLastTab() =>
        AqualityServices.Browser.Windows().SwitchToLast();

    public static void CloseCurrentTabAndReturnToMain()
    {
        var handles = AqualityServices.Browser.Windows().Handles;

        if (handles.Count >= WebConstants.BrowserTabs.MinTabCountToCloseExtra)
        {
            AqualityServices.Browser.Windows().Close();
        }

        if (!string.IsNullOrEmpty(mainWindowHandle) && handles.Contains(mainWindowHandle))
        {
            AqualityServices.Browser.Windows().SwitchTo(mainWindowHandle);
        }
        else
        {
            AqualityServices.Browser.Windows().SwitchTo(WebConstants.BrowserTabs.FirstTabIndex);
        }

        AqualityServices.Browser.WaitForPageToLoad();
    }
}
