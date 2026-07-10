namespace UnionReportingApi.Constants;

public static class DirectoryPaths
{
    public const string ResourcesFolder = "Resources";

    public const string AppSettingsFileName = "appsettings.json";

    public const string AppSettingsLocalFileName = "appsettings.local.json";

    public const string AppSettingsSectionName = "UnionReporting";

    public const string TestDataFileName = "testdata.json";

    public static string BasePath =>
        AppContext.BaseDirectory;

    public static string ResourcesFolderPath =>
        Path.Join(BasePath, ResourcesFolder);

    public static string AppSettingsPath =>
        Path.Join(BasePath, AppSettingsFileName);

    public static string AppSettingsLocalPath =>
        Path.Join(BasePath, AppSettingsLocalFileName);

    public static string TestDataPath =>
        Path.Join(ResourcesFolderPath, TestDataFileName);
}
