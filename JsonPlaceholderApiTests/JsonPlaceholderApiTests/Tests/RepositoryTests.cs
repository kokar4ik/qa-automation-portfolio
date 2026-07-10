using FluentAssertions;
using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Tests;

public class RepositoryTests : DatabaseTestBase
{
    private readonly List<long> _createdTestIds = [];

    [TearDown]
    public async Task TearDown()
    {
        foreach (var testId in _createdTestIds)
        {
            await TestRepository.DeleteAsync(testId);
        }

        _createdTestIds.Clear();
    }

    [Test]
    public async Task StatusRepositoryGetAllReturnsExistingStatuses()
    {
        var statuses = await StatusRepository.GetAllAsync();

        statuses.Should().HaveCount(3);
        statuses.Select(status => status.Name)
            .Should()
            .BeEquivalentTo(["PASSED", "FAILED", "SKIPPED"]);
    }

    [Test]
    public async Task TestRepositoryGetByRepeatingDigitIdsReturnsMatchingTests()
    {
        var tests = await TestRepository.GetByRepeatingDigitIdsAsync(DatabaseTestData.RepeatingDigitTestsLimit);

        tests.Should().NotBeEmpty();
        tests.Should().HaveCountLessThanOrEqualTo(DatabaseTestData.RepeatingDigitTestsLimit);
        tests.Select(test => test.Id.ToString())
            .Should()
            .OnlyContain(id => HasAdjacentRepeatingDigits(id));
    }

    private static bool HasAdjacentRepeatingDigits(string id)
    {
        for (var index = 0; index < id.Length - 1; index++)
        {
            if (id[index] == id[index + 1])
            {
                return true;
            }
        }

        return false;
    }

    [Test]
    public async Task TestRepositoryCreateUpdateAndDeleteWorksCorrectly()
    {
        var project = await ProjectRepository.GetOrCreateAsync(DatabaseTestData.ProjectName);
        var author = await AuthorRepository.GetOrCreateAsync(
            DatabaseTestData.AuthorName,
            DatabaseTestData.AuthorLogin,
            DatabaseTestData.AuthorEmail);
        var sessionId = await SessionRepository.CreateAsync(new SessionRecord
        {
            SessionKey = Guid.NewGuid().ToString("N"),
            CreatedTime = DateTime.UtcNow,
            BuildNumber = 1
        });

        var startTime = DateTime.UtcNow;
        var testId = await TestRepository.CreateAsync(new TestRecord
        {
            Name = "Repository smoke test",
            StatusId = (int)TestStatus.Passed,
            MethodName = nameof(TestRepositoryCreateUpdateAndDeleteWorksCorrectly),
            ProjectId = project.Id,
            SessionId = sessionId,
            StartTime = startTime,
            EndTime = startTime.AddSeconds(DatabaseTestData.CreateTestDurationSeconds),
            Env = Environment.MachineName,
            Browser = "dotnet",
            AuthorId = author.Id
        });
        _createdTestIds.Add(testId);

        var createdTest = await TestRepository.GetByIdAsync(testId);
        createdTest.Should().NotBeNull();
        createdTest!.Name.Should().Be("Repository smoke test");
        createdTest.StatusId.Should().Be((int)TestStatus.Passed);

        createdTest.StatusId = (int)TestStatus.Failed;
        createdTest.EndTime = startTime.AddSeconds(DatabaseTestData.UpdateTestDurationSeconds);
        await TestRepository.UpdateAsync(createdTest);

        var updatedTest = await TestRepository.GetByIdAsync(testId);
        updatedTest!.StatusId.Should().Be((int)TestStatus.Failed);
        updatedTest.EndTime.Should().BeCloseTo(
            startTime.AddSeconds(DatabaseTestData.UpdateTestDurationSeconds),
            TimeSpan.FromSeconds(DatabaseTestData.CreateTestDurationSeconds));

        await TestRepository.DeleteAsync(testId);
        _createdTestIds.Remove(testId);

        var deletedTest = await TestRepository.GetByIdAsync(testId);
        deletedTest.Should().BeNull();
    }

    [Test]
    public async Task TestRepositoryCopyCreatesRecordWithNewForeignKeys()
    {
        var sourceTests = await TestRepository.GetByRepeatingDigitIdsAsync(DatabaseTestData.CopySourceTestsLimit);
        sourceTests.Should().NotBeEmpty();

        var project = await ProjectRepository.GetOrCreateAsync(DatabaseTestData.ProjectName);
        var author = await AuthorRepository.GetOrCreateAsync(
            DatabaseTestData.AuthorName,
            DatabaseTestData.AuthorLogin,
            DatabaseTestData.AuthorEmail);
        var sessionId = await SessionRepository.CreateAsync(new SessionRecord
        {
            SessionKey = Guid.NewGuid().ToString("N"),
            CreatedTime = DateTime.UtcNow,
            BuildNumber = 1
        });

        var copiedTest = await TestRepository.CopyAsync(
            sourceTests[0],
            project.Id,
            author.Id,
            sessionId);

        _createdTestIds.Add(copiedTest.Id);

        copiedTest.Id.Should().NotBe(sourceTests[0].Id);
        copiedTest.Name.Should().Be(sourceTests[0].Name);
        copiedTest.ProjectId.Should().Be(project.Id);
        copiedTest.AuthorId.Should().Be(author.Id);
        copiedTest.SessionId.Should().Be(sessionId);
    }
}