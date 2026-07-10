namespace YandexDiskApi.Helpers;

public static class TestDataGenerator
{
    public static string CreateFileName(string extension) =>
        $"autotest_{Guid.NewGuid():N}.{extension.TrimStart('.')}";

    public static string CreateRandomText() =>
        $"{Guid.NewGuid():N}";
}
