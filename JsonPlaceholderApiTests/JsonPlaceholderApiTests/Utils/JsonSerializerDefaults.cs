using System.Text.Json;

namespace JsonPlaceholderApiTests.Utils;

public static class JsonSerializerDefaults
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };
}