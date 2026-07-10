using Aquality.Selenium.Browsers;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using UnionReportingApi.Constants;

namespace UnionReportingApi.Forms;

public class AllTestsForm : Form
{
    private static readonly By TestsTableLocator = By.Id(WebConstants.ElementIds.AllTestsTable);

    private static readonly By DataRowsLocator =
        By.XPath(".//tr[td/a]");

    private const string TestLinkXPathTemplate =
        "//table[@id='{0}']//a[normalize-space()='{1}']";

    public AllTestsForm() : base(By.Id(WebConstants.ElementIds.Pie), "Страница тестов проекта")
    {
    }

    public bool IsTestsTableDisplayed()
    {
        var table = AqualityServices.Browser.Driver
            .FindElements(TestsTableLocator)
            .FirstOrDefault();

        return table is not null && table.Displayed;
    }

    public IReadOnlyList<UiTestRow> GetFirstPageTestRows()
    {
        var table = AqualityServices.Browser.Driver.FindElement(TestsTableLocator);
        var columnIndexes = BuildColumnIndexMap(table);
        var rows = table.FindElements(DataRowsLocator);

        return rows
            .Select(row => ReadRow(row, columnIndexes))
            .ToList();
    }

    public bool IsTestDisplayed(string testName) =>
        GetFirstPageTestRows()
            .Any(row => string.Equals(row.Name, testName, StringComparison.Ordinal));

    public void OpenTest(string testName)
    {
        ElementFactory.GetLink(
                By.XPath(string.Format(
                    TestLinkXPathTemplate,
                    WebConstants.ElementIds.AllTestsTable,
                    testName)),
                $"Тест {testName}")
            .Click();
        AqualityServices.Browser.WaitForPageToLoad();
    }

    private static Dictionary<string, int> BuildColumnIndexMap(IWebElement table)
    {
        var headers = table.FindElements(By.TagName(WebConstants.Html.TableHeaderCellTag));
        var columnIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < headers.Count; index++)
        {
            columnIndexes[headers[index].Text.Trim()] = index;
        }

        return columnIndexes;
    }

    private static UiTestRow ReadRow(IWebElement row, IReadOnlyDictionary<string, int> columnIndexes)
    {
        var cells = row.FindElements(By.TagName(WebConstants.Html.TableDataCellTag));

        return new UiTestRow
        {
            Name = GetCellText(cells, columnIndexes, WebConstants.AllTestsTableColumns.TestName),
            Method = GetCellText(cells, columnIndexes, WebConstants.AllTestsTableColumns.TestMethod),
            Status = GetCellText(cells, columnIndexes, WebConstants.AllTestsTableColumns.LatestTestResult),
            StartTime = GetCellText(cells, columnIndexes, WebConstants.AllTestsTableColumns.LatestTestStartTime),
        };
    }

    private static string GetCellText(
        IReadOnlyList<IWebElement> cells,
        IReadOnlyDictionary<string, int> columnIndexes,
        string columnName)
    {
        if (!columnIndexes.TryGetValue(columnName, out var columnIndex) || columnIndex >= cells.Count)
        {
            return string.Empty;
        }

        return cells[columnIndex].Text.Trim();
    }
}
