namespace UnionReportingApi.Constants;

public static class WebConstants
{
    public static class Paths
    {
        public const string ProjectsPage = "projects";
    }

    public static class Cookies
    {
        public const string Token = "token";

        public const string Path = "/";

        public const int LifetimeDays = 1;
    }

    public static class ElementIds
    {
        public const string AllTestsTable = "allTests";

        public const string Pie = "pie";
    }

    public static class AllTestsTableColumns
    {
        public const string TestName = "Test name";

        public const string TestMethod = "Test method";

        public const string LatestTestResult = "Latest test result";

        public const string LatestTestStartTime = "Latest test start time";
    }

    public static class TestInfoFields
    {
        public const string ProjectName = "Project name";

        public const string TestName = "Test name";

        public const string TestMethodName = "Test method name";

        public const string Status = "Status";

        public const string Environment = "Environment";

        public const string Browser = "Browser";
    }

    public static class Html
    {
        public const string SourceAttribute = "src";

        public const string TableDataCellTag = "td";

        public const string TableHeaderCellTag = "th";
    }

    public static class BrowserTabs
    {
        public const int MinTabCountToCloseExtra = 2;

        public const int FirstTabIndex = 0;
    }
}
