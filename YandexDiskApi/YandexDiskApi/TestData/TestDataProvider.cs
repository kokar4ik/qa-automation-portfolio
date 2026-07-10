using System.Text.Json;

namespace YandexDiskApi.TestData;

public sealed class TestDataProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static TestDataProvider? instance;

    private TestDataProvider(ScenarioTestData data) => Data = data;

    public ScenarioTestData Data { get; }

    public TimeoutSettings Timeouts => Data.Timeouts;

    public static TestDataProvider Instance =>
        instance ??= Load();

    public static TestDataProvider Load(string? basePath = null)
    {
        basePath ??= AppContext.BaseDirectory;
        var filePath = Path.Combine(basePath, "Resources", "testdata.json");
        var json = File.ReadAllText(filePath);
        var data = JsonSerializer.Deserialize<ScenarioTestData>(json, JsonOptions)
            ?? throw new InvalidOperationException("Не удалось загрузить testdata.json.");

        instance = new TestDataProvider(data);
        return instance;
    }

    public static string ResolveResourcePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new InvalidOperationException("Путь к ресурсу не задан в testdata.json.");
        }

        var outputPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        throw new FileNotFoundException(
            $"Файл ресурса не найден: {outputPath}. Пересоберите проект (dotnet build).");
    }
}

public sealed class ScenarioTestData
{
    public string TextFileExtension { get; init; } = "txt";

    public string ImageFileExtension { get; init; } = "jpg";

    public string ImageResourcePath { get; init; } = string.Empty;

    public TimeoutSettings Timeouts { get; init; } = new();
}

public sealed class TimeoutSettings
{
    public int ConditionSeconds { get; init; } = 60;

    public int PollingSeconds { get; init; } = 1;

    public int LoginSeconds { get; init; } = 30;

    public int FileDisplayedSeconds { get; init; } = 12;

    public int FileDisappearedSeconds { get; init; } = 20;

    public int DocumentOpenSeconds { get; init; } = 15;

    public int DocumentTextSeconds { get; init; } = 60;

    public int ImageDownloadSeconds { get; init; } = 30;

    public int TrashSeconds { get; init; } = 20;

    public int PasswordPageSeconds { get; init; } = 10;

    public int FingerprintSeconds { get; init; } = 120;

    public int ManualConfirmationSeconds { get; init; } = 180;
}
