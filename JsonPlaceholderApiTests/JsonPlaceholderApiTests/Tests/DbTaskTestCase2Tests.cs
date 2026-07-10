using FluentAssertions;
using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Tests;

public class DbTaskTestCase2Tests : DbTaskTestBase
{
    private readonly List<long> _copiedTestIds = [];

    [TearDown]
    public async Task TearDown()
    {
        foreach (var testId in _copiedTestIds)
        {
            await TestRepository.DeleteAsync(testId);
        }

        _copiedTestIds.Clear();
    }

    [Test]
    public async Task TestCase2CopySimulateRunUpdateAndCleanup()
    {
        var sourceTests = await TestRepository.GetByRepeatingDigitIdsAsync(DatabaseTestData.RepeatingDigitTestsLimit);
        sourceTests.Should().NotBeEmpty();
        sourceTests.Should().HaveCountLessThanOrEqualTo(DatabaseTestData.RepeatingDigitTestsLimit);

        var (project, author, sessionId) = await CreateTestContextAsync();

        foreach (var sourceTest in sourceTests)
        {
            var copiedTest = await TestRepository.CopyAsync(
                sourceTest,
                project.Id,
                author.Id,
                sessionId);

            _copiedTestIds.Add(copiedTest.Id);

            var simulatedTest = TestRunSimulator.SimulateRun(copiedTest);
            await TestRepository.UpdateAsync(simulatedTest);

            var updatedTest = await TestRepository.GetByIdAsync(copiedTest.Id);
            updatedTest.Should().NotBeNull();
            updatedTest!.StatusId.Should().BeOneOf(
                (int)TestStatus.Passed,
                (int)TestStatus.Failed,
                (int)TestStatus.Skipped);
            updatedTest.StartTime.Should().NotBeNull();
            updatedTest.EndTime.Should().BeAfter(updatedTest.StartTime!.Value);
            updatedTest.ProjectId.Should().Be(project.Id);
            updatedTest.AuthorId.Should().Be(author.Id);
        }

        foreach (var sourceTest in sourceTests)
        {
            var originalTest = await TestRepository.GetByIdAsync(sourceTest.Id);
            originalTest.Should().NotBeNull();
        }

        foreach (var copiedTestId in _copiedTestIds)
        {
            var copiedTest = await TestRepository.GetByIdAsync(copiedTestId);
            copiedTest.Should().NotBeNull();
        }
    }
}
