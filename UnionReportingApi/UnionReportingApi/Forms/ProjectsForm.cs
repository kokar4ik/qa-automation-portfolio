using Aquality.Selenium.Browsers;
using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using UnionReportingApi.Helpers;

namespace UnionReportingApi.Forms;

public class ProjectsForm : Form
{
    private const string ProjectLinkXPathTemplate =
        "//a[contains(@class,'list-group-item') and normalize-space()='{0}']";

    public ProjectsForm()
        : base(
            By.XPath("//div[contains(@class,'panel-heading') and contains(normalize-space(), 'Available projects')]"),
            "Страница проектов")
    {
    }

    private IButton AddProjectLink =>
        ElementFactory.GetButton(
            By.XPath("//a[contains(@class,'btn') and contains(normalize-space(), '+Add')]"),
            "+Add");

    private ILabel FooterText =>
        ElementFactory.GetLabel(By.CssSelector("footer .footer-text"), "Футер");

    public string GetFooterText() =>
        FooterText.GetText();

    public void ClickAddProject()
    {
        BrowserTabsHelper.RememberMainTab();
        AddProjectLink.Click();
    }

    public void OpenProject(string projectName)
    {
        ElementFactory.GetLink(
                By.XPath(string.Format(ProjectLinkXPathTemplate, projectName)),
                $"Проект {projectName}")
            .Click();
        AqualityServices.Browser.WaitForPageToLoad();
    }

    public bool IsProjectDisplayed(string projectName) =>
        ElementFactory.GetLink(
            By.XPath(string.Format(ProjectLinkXPathTemplate, projectName)),
            $"Проект {projectName}").State.IsDisplayed;

    public void RefreshPage()
    {
        AqualityServices.Browser.Refresh();
        AqualityServices.Browser.WaitForPageToLoad();
    }

    public void NavigateBack()
    {
        AqualityServices.Browser.GoBack();
        AqualityServices.Browser.WaitForPageToLoad();
    }
}
