using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Database.Utils;

public class TestRunSimulator
{
    private const int RunDurationSeconds = 60;

    private static readonly TestStatus[] PossibleStatuses =
    [
        TestStatus.Passed,
        TestStatus.Failed,
        TestStatus.Skipped
    ];

    public TestRecord SimulateRun(TestRecord test)
    {
        var startTime = DateTime.UtcNow.AddSeconds(-RunDurationSeconds);

        test.StartTime = startTime;
        test.EndTime = startTime.AddSeconds(RunDurationSeconds);
        test.StatusId = (int)PossibleStatuses[Random.Shared.Next(PossibleStatuses.Length)];

        return test;
    }
}
