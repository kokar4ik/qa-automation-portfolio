using System.Text.Json;
using JsonPlaceholderApiTests.Constants;
using JsonPlaceholderApiTests.Models;

namespace JsonPlaceholderApiTests.Utils;

public static class TestDataProvider
{
    private static readonly string DataDirectoryPath = Path.Combine(
        AppContext.BaseDirectory,
        ConfigurationConstants.DataDirectoryName);

    public static User LoadExpectedUser(string fileName)
    {
        var filePath = Path.Combine(DataDirectoryPath, fileName);
        var json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<User>(json, JsonSerializerDefaults.Options)
            ?? throw new InvalidOperationException($"Не удалось загрузить пользователя из '{filePath}'.");
    }
}