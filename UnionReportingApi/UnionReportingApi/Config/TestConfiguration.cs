using UnionReportingApi.Constants;
using UnionReportingApi.Utils;

namespace UnionReportingApi.Config;

public sealed class TestConfiguration
{
    private static TestConfiguration? instance;

    public required string BaseUrl { get; init; }

    public required string ApiBaseUrl { get; init; }

    public required string Login { get; init; }

    public required string Password { get; init; }

    public required int VariantId { get; init; }

    public static TestConfiguration Instance =>
        instance ??= Load();

    private static TestConfiguration Load()
    {
        instance = JsonConvertHelper.DeserializeSectionFromFiles<TestConfiguration>(
            DirectoryPaths.AppSettingsSectionName,
            DirectoryPaths.AppSettingsPath,
            DirectoryPaths.AppSettingsLocalPath);

        return instance;
    }
}
