using Aquality.Selenium.Browsers;

namespace YandexDiskApi.Helpers;

public static class BrowserTabsHelper
{
    private static string? mainWindowHandle;

    public static void RememberMainTab() =>
        mainWindowHandle = AqualityServices.Browser.Windows().CurrentHandle;

    public static void SwitchToLastTab()
    {
        var handles = AqualityServices.Browser.Windows().Handles;
        AqualityServices.Browser.Windows().SwitchTo(handles[^1]);
    }

    public static void CloseCurrentTabAndReturnToMain()
    {
        var handles = AqualityServices.Browser.Windows().Handles;

        if (handles.Count > 1)
        {
            AqualityServices.Browser.Windows().Close();
        }

        if (!string.IsNullOrEmpty(mainWindowHandle) && handles.Contains(mainWindowHandle))
        {
            AqualityServices.Browser.Windows().SwitchTo(mainWindowHandle);
        }
        else
        {
            AqualityServices.Browser.Windows().SwitchTo(0);
        }

        AqualityServices.Browser.WaitForPageToLoad();
    }
}
