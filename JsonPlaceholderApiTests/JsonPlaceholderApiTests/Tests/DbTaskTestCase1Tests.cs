using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Tests;

public class DbTaskTestCase1Tests : DbTaskTestBase
{
    [Test]
    public async Task TestCase1RunApiScenarioAndSaveResultToDatabase()
    {
        var startTime = DateTime.UtcNow;

        await JsonPlaceholderScenarioRunner.RunSixStepsAsync(PostsApi, UsersApi, TestData);

        var endTime = DateTime.UtcNow;
        var (project, author, sessionId) = await CreateTestContextAsync();

        var testName = TestContext.CurrentContext.Test.Name;
        var testId = await TestRepository.CreateAsync(new TestRecord
        {
            Name = testName,
            StatusId = (int)TestStatus.Passed,
            MethodName = nameof(TestCase1RunApiScenarioAndSaveResultToDatabase),
            ProjectId = project.Id,
            SessionId = sessionId,
            StartTime = startTime,
            EndTime = endTime,
            Env = Environment.MachineName,
            Browser = "dotnet",
            AuthorId = author.Id
        });

        var savedTest = await TestRepository.GetByIdAsync(testId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(savedTest, Is.Not.Null);
            Assert.That(savedTest!.Name, Is.EqualTo(testName));
            Assert.That(savedTest.StatusId, Is.EqualTo((int)TestStatus.Passed));
            Assert.That(savedTest.ProjectId, Is.EqualTo(project.Id));
            Assert.That(savedTest.AuthorId, Is.EqualTo(author.Id));
            Assert.That(savedTest.SessionId, Is.EqualTo(sessionId));
            Assert.That(savedTest.MethodName, Is.EqualTo(nameof(TestCase1RunApiScenarioAndSaveResultToDatabase)));
        }
    }
}
