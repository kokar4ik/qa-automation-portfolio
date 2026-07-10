using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;

namespace UnionReportingApi.Forms;

public class AddProjectForm : Form
{
    public AddProjectForm() : base(By.Id("projectName"), "Добавление проекта")
    {
    }

    private ITextBox ProjectNameTextBox =>
        ElementFactory.GetTextBox(By.Id("projectName"), "Project Name");

    private IButton SaveButton =>
        ElementFactory.GetButton(By.XPath("//*[@type='submit']"), "Save Project");

    private ILabel SuccessAlert =>
        ElementFactory.GetLabel(By.ClassName("alert-success"), "Сообщение об успехе");

    public void TypeProjectName(string projectName) =>
        ProjectNameTextBox.ClearAndType(projectName);

    public void ClickSave() =>
        SaveButton.Click();

    public bool IsSuccessAlertDisplayed() =>
        SuccessAlert.State.IsDisplayed;
}
