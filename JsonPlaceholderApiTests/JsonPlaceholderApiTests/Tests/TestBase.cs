using Microsoft.Extensions.Configuration;
using JsonPlaceholderApiTests.Api;
using JsonPlaceholderApiTests.Configuration;
using JsonPlaceholderApiTests.Constants;
using JsonPlaceholderApiTests.Utils;

namespace JsonPlaceholderApiTests.Tests;

public abstract class TestBase
{
    protected ApiSettings Settings = null!;
    protected TestDataSettings TestData = null!;
    protected PostsApi PostsApi = null!;
    protected UsersApi UsersApi = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TestLogger.Initialize();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(TestContext.CurrentContext.TestDirectory)
            .AddJsonFile(ConfigurationConstants.AppSettingsFileName, optional: false)
            .Build();

        Settings = configuration.GetRequiredSection<ApiSettings>(ConfigurationConstants.ApiSettingsSection);
        TestData = configuration.GetRequiredSection<TestDataSettings>(ConfigurationConstants.TestDataSection);

        PostsApi = new PostsApi(Settings);
        UsersApi = new UsersApi(Settings);

        TestLogger.LogInfo("TestBase инициализирован. Конфигурация загружена.");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        PostsApi.Dispose();
        PostsApi = null!;

        UsersApi.Dispose();
        UsersApi = null!;

        TestLogger.LogInfo("TestBase завершил работу.");
        TestLogger.Shutdown();
    }
}
