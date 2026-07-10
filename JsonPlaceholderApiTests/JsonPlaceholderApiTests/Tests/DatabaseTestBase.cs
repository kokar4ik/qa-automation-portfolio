using JsonPlaceholderApiTests.Configuration;
using JsonPlaceholderApiTests.Constants;
using JsonPlaceholderApiTests.Database;
using JsonPlaceholderApiTests.Database.Repositories;
using JsonPlaceholderApiTests.Utils;
using Microsoft.Extensions.Configuration;

namespace JsonPlaceholderApiTests.Tests;

public abstract class DatabaseTestBase
{
    protected DatabaseConnection DatabaseConnection = null!;
    protected StatusRepository StatusRepository = null!;
    protected ProjectRepository ProjectRepository = null!;
    protected AuthorRepository AuthorRepository = null!;
    protected SessionRepository SessionRepository = null!;
    protected TestRepository TestRepository = null!;
    protected DatabaseTestDataSettings DatabaseTestData = null!;

    [OneTimeSetUp]
    public void OneTimeDatabaseSetUp()
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
    }

    [OneTimeTearDown]
    public void OneTimeDatabaseTearDown()
    {
        DatabaseConnection.Dispose();
    }
}