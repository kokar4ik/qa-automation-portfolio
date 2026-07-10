using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnionReportingApi.Utils;

public static class JsonConvertHelper
{
    private const string NullContentPlaceholder = "<null>";
    public static T DeserializeFromFile<T>(string path)
    {
        var json = File.ReadAllText(path);
        return Deserialize<T>(json, path);
    }

    public static T DeserializeSectionFromFiles<T>(string sectionName, params string[] paths)
    {
        var merged = LoadMergedJson(paths);
        var section = merged[sectionName];

        if (section is null || section.Type == JTokenType.Null)
        {
            throw new InvalidOperationException($"В appsettings отсутствует секция '{sectionName}'.");
        }

        try
        {
            return section.ToObject<T>()
                ?? throw new InvalidOperationException(GetNullDeserializationMessage(typeof(T), sectionName));
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(GetInvalidJsonMessage(typeof(T), sectionName), ex);
        }
    }

    public static T Deserialize<T>(string json, string? source = null)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json)
                ?? throw new InvalidOperationException(GetNullDeserializationMessage(typeof(T), source));
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(GetInvalidJsonMessage(typeof(T), source), ex);
        }
    }

    public static string GetContentSnippet(string? content, int maxLength)
    {
        if (content is null)
        {
            return NullContentPlaceholder;
        }

        return content.Length <= maxLength
            ? content
            : content[..maxLength];
    }

    private static JObject LoadMergedJson(params string[] paths)
    {
        var merged = new JObject();

        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                continue;
            }

            var current = DeserializeFromFile<JObject>(path);
            merged.Merge(
                current,
                new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Replace,
                    MergeNullValueHandling = MergeNullValueHandling.Merge,
                });
        }

        return merged;
    }

    private static string GetInvalidJsonMessage(Type targetType, string? source) =>
        source is null
            ? $"Некорректный JSON при десериализации в {targetType.Name}."
            : $"Некорректный JSON в '{source}' при десериализации в {targetType.Name}.";

    private static string GetNullDeserializationMessage(Type targetType, string? source) =>
        source is null
            ? $"Десериализация в {targetType.Name} вернула null."
            : $"Десериализация в {targetType.Name} вернула null. source='{source}'.";
}
