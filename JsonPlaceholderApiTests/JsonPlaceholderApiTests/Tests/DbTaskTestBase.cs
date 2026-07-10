using JsonPlaceholderApiTests.Configuration;
using JsonPlaceholderApiTests.Constants;
using JsonPlaceholderApiTests.Database;
using JsonPlaceholderApiTests.Database.Models;
using JsonPlaceholderApiTests.Database.Repositories;
using JsonPlaceholderApiTests.Database.Utils;
using JsonPlaceholderApiTests.Utils;
using Microsoft.Extensions.Configuration;

namespace JsonPlaceholderApiTests.Tests;

public abstract class DbTaskTestBase : TestBase
{
    protected DatabaseConnection DatabaseConnection = null!;
    protected StatusRepository StatusRepository = null!;
    protected ProjectRepository ProjectRepository = null!;
    protected AuthorRepository AuthorRepository = null!;
    protected SessionRepository SessionRepository = null!;
    protected TestRepository TestRepository = null!;
    protected TestRunSimulator TestRunSimulator = null!;
    protected DatabaseTestDataSettings DatabaseTestData = null!;

    [OneTimeSetUp]
    public void OneTimeDbTaskSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(TestContext.CurrentContext.TestDirectory)
            .AddJsonFile(ConfigurationConstants.AppSettingsFileName, optional: false)
            .Build();

        var databaseSettings = configuration.GetRequiredSection<DatabaseSettings>(
            ConfigurationConstants.DatabaseSection);
        DatabaseTestData = configuration.GetRequiredSection<DatabaseTestDataSettings>(
            ConfigurationConstants.DatabaseTestDataSection);

        DatabaseConnection = new DatabaseConnection(databaseSettings);
        DatabaseConnection.Open();
        StatusRepository = new StatusRepository(DatabaseConnection);
        ProjectRepository = new ProjectRepository(DatabaseConnection);
        AuthorRepository = new AuthorRepository(DatabaseConnection);
        SessionRepository = new SessionRepository(DatabaseConnection);
        TestRepository = new TestRepository(DatabaseConnection);
        TestRunSimulator = new TestRunSimulator();
    }

    [OneTimeTearDown]
    public void OneTimeDbTaskTearDown()
    {
        DatabaseConnection.Dispose();
    }

    protected async Task<(ProjectRecord Project, AuthorRecord Author, long SessionId)> CreateTestContextAsync()
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

        return (project, author, sessionId);
    }
}
