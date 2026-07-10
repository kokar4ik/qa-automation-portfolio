using Microsoft.Extensions.Configuration;

namespace YandexDiskApi.Config;

public sealed class TestConfiguration
{
    private static TestConfiguration? instance;

    private TestConfiguration(IConfiguration configuration)
    {
        BaseUrl = configuration["YandexDisk:BaseUrl"] ?? string.Empty;
        ApiBaseUrl = configuration["YandexDisk:ApiBaseUrl"] ?? string.Empty;
        OAuthToken = configuration["YandexDisk:OAuthToken"] ?? string.Empty;
        Login = configuration["YandexDisk:Login"] ?? string.Empty;
        Password = configuration["YandexDisk:Password"] ?? string.Empty;
    }

    public string BaseUrl { get; }

    public string ApiBaseUrl { get; }

    public string OAuthToken { get; }

    public string Login { get; }

    public string Password { get; }

    public static TestConfiguration Instance =>
        instance ??= Load();

    public static TestConfiguration Load(string? basePath = null)
    {
        basePath ??= AppContext.BaseDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        instance = new TestConfiguration(configuration);
        return instance;
    }
}