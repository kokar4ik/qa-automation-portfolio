using UnionReportingApi.Constants;
using UnionReportingApi.Utils;

namespace UnionReportingApi.TestData;

public sealed class TestDataProvider
{
    private static TestDataProvider? instance;

    private TestDataProvider(ScenarioTestData data) => Data = data;

    public ScenarioTestData Data { get; }

    public static TestDataProvider Instance =>
        instance ??= Load();

    private static TestDataProvider Load()
    {
        var data = JsonConvertHelper.DeserializeFromFile<ScenarioTestData>(DirectoryPaths.TestDataPath);

        instance = new TestDataProvider(data);
        return instance;
    }
}

public sealed class ScenarioTestData
{
    public required long NexageProjectId { get; init; }

    public required string NexageProjectName { get; init; }

    public required string ProjectNamePrefix { get; init; }

    public required string TestNamePrefix { get; init; }

    public required string TestMethodName { get; init; }

    public required string DefaultLogContent { get; init; }

    public required string DefaultTestStatus { get; init; }

    public required int TestsPerPage { get; init; }

    public required int AjaxRefreshTimeoutSeconds { get; init; }

    public required int ApiFormatRetryCount { get; init; }

    public required int ConditionTimeoutSeconds { get; init; }

    public required int PollingIntervalSeconds { get; init; }

    public required int ProjectSuccessAlertTimeoutSeconds { get; init; }

    public required string ExpectedFooterVersionPrefix { get; init; }

    public required string DefaultBrowser { get; init; }

    public required string DefaultEnvironment { get; init; }
}
