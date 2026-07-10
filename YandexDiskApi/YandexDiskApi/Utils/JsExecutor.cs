using Aquality.Selenium.Browsers;
using OpenQA.Selenium;

namespace YandexDiskApi.Utils;

public static class JsExecutor
{
    private const string ScrollIntoViewScript =
        "arguments[0].scrollIntoView({block: 'center', inline: 'nearest'});";

    private const string ScrollTrashNavigationScript =
        """
        const navigation = document.querySelector('[class*="LeftColumnNavigation"]');
        if (navigation && navigation.scrollHeight > navigation.clientHeight) {
            navigation.scrollTop = navigation.scrollHeight;
        }

        arguments[0].scrollIntoView({ block: 'center', inline: 'nearest' });
        """;

    public static void ScrollIntoView(IWebElement element) =>
        Execute(ScrollIntoViewScript, element);

    public static void ScrollTrashIntoView(IWebElement trashElement) =>
        Execute(ScrollTrashNavigationScript, trashElement);

    public static string ReadDocumentParagraphText() =>
        AqualityServices.Browser.ExecuteScript<string>(
            """
            function readFromRoot(root) {
                if (!root) {
                    return '';
                }

                const hosts = root.querySelectorAll('div[class*="__page-"]');
                for (const host of hosts) {
                    if (!host.shadowRoot) {
                        continue;
                    }

                    const paragraph = host.shadowRoot.querySelector('p.mg1');
                    if (paragraph) {
                        return (paragraph.textContent || '').trim();
                    }
                }

                return '';
            }

            let text = readFromRoot(document);
            if (text) {
                return text;
            }

            for (const iframe of document.querySelectorAll('iframe')) {
                try {
                    text = readFromRoot(iframe.contentDocument);
                    if (text) {
                        return text;
                    }
                } catch (e) {
                }
            }

            return '';
            """) ?? string.Empty;

    private static void Execute(string script, IWebElement element)
    {
        var driver = AqualityServices.Browser.Driver;
        ((IJavaScriptExecutor)driver).ExecuteScript(script, element);
    }
}
