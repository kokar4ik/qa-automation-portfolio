using UnionReportingApi.Forms;
using UnionReportingApi.Helpers;
using UnionReportingApi.TestData;
using UnionReportingApi.Utils;

namespace UnionReportingApi.Steps;

public class ProjectSteps
{
    public string CreateProjectInNewTab(string? projectName = null)
    {
        var testData = TestDataProvider.Instance.Data;
        projectName ??= $"{testData.ProjectNamePrefix}{Guid.NewGuid():N}";

        var projectsForm = new ProjectsForm();
        projectsForm.ClickAddProject();
        BrowserTabsHelper.SwitchToLastTab();

        var addProjectForm = new AddProjectForm();
        addProjectForm.TypeProjectName(projectName);
        addProjectForm.ClickSave();

        try
        {
            ConditionalWaitHelper.WaitForTrue(
                addProjectForm.IsSuccessAlertDisplayed,
                "Ожидается сообщение об успешном сохранении проекта.",
                testData.ProjectSuccessAlertTimeoutSeconds);
        }
        catch (TimeoutException)
        {
        }

        BrowserTabsHelper.CloseCurrentTabAndReturnToMain();
        projectsForm.RefreshPage();

        return projectName;
    }
}