using Aquality.Selenium.Browsers;
using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using UnionReportingApi.Constants;

namespace UnionReportingApi.Forms;

public class TestInfoForm : Form
{
    private const string CommonInfoValueXPathTemplate =
        "//h4[contains(@class,'list-group-item-heading') and normalize-space()='{0}']/following-sibling::p[contains(@class,'list-group-item-text')][1]";

    private const string LogCellsXPath =
        "//div[contains(@class,'panel-heading') and normalize-space()='Logs']/following::table//td";

    public TestInfoForm() : base(By.XPath("//*[normalize-space()='Logs']"), "Страница теста")
    {
    }

    private ILabel AttachmentThumbnail =>
        ElementFactory.GetLabel(By.ClassName("thumbnail"), "Attachment thumbnail");

    public IReadOnlyList<string> GetLogLines()
    {
        var cells = AqualityServices.Browser.Driver.FindElements(By.XPath(LogCellsXPath));
        return cells.Select(cell => cell.Text.Trim()).Where(text => !string.IsNullOrEmpty(text)).ToList();
    }

    public string GetCommonInfoValue(string heading) =>
        ElementFactory.GetLabel(
            By.XPath(string.Format(CommonInfoValueXPathTemplate, heading)),
            heading).GetText();

    public string GetAttachmentImageSource()
    {
        if (!AttachmentThumbnail.State.IsExist)
        {
            return string.Empty;
        }

        return AttachmentThumbnail.GetAttribute(WebConstants.Html.SourceAttribute) ?? string.Empty;
    }
}
