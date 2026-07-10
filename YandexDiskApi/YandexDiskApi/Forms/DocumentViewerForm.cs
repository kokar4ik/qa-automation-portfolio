using Aquality.Selenium.Browsers;
using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using YandexDiskApi.Utils;

namespace YandexDiskApi.Forms;

public class DocumentViewerForm : Form
{
    private const string FormName = "Просмотр документа";
    private const string DocumentTitleElementName = "Заголовок документа";
    private const string TextContentDomProperty = "textContent";

    private static readonly By DocumentPageHostLocator = By.CssSelector("div[class*='__page-']");
    private static readonly By DocumentParagraphLocator = By.CssSelector("p.mg1");
    private static readonly By DocumentTitleLocator =
        By.CssSelector("div[class*='titleWrapper'] h2[class*='title_']");

    public DocumentViewerForm()
        : base(DocumentPageHostLocator, FormName)
    {
    }

    private ILabel DocumentTitle =>
        ElementFactory.GetLabel(DocumentTitleLocator, DocumentTitleElementName);

    public bool IsTitleDisplayed(string fileName)
    {
        if (!DocumentTitle.State.IsDisplayed)
        {
            return false;
        }

        return DocumentTitle.GetText().Contains(fileName, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsTextLoaded() =>
        !string.IsNullOrWhiteSpace(GetRawText());

    public string GetRawText()
    {
        var fromScript = JsExecutor.ReadDocumentParagraphText();
        if (!string.IsNullOrWhiteSpace(fromScript))
        {
            return fromScript;
        }

        return ReadFromShadowRoot();
    }

    private static string ReadFromShadowRoot()
    {
        var driver = AqualityServices.Browser.Driver;

        foreach (var host in driver.FindElements(DocumentPageHostLocator))
        {
            try
            {
                var text = ReadParagraphFromShadowHost(host);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
            catch (NoSuchShadowRootException)
            {
            }
        }

        return string.Empty;
    }

    private static string ReadParagraphFromShadowHost(IWebElement host)
    {
        var shadowRoot = host.GetShadowRoot();
        var paragraph = shadowRoot.FindElement(DocumentParagraphLocator);
        return paragraph.GetDomProperty(TextContentDomProperty)?.Trim() ?? string.Empty;
    }
}