using System.Text.Json.Serialization;

namespace YandexDiskApi.Api.Models;

public sealed class TrashResourcesResponse
{
    [JsonPropertyName("_embedded")]
    public TrashEmbedded? Embedded { get; init; }
}

public sealed class TrashEmbedded
{
    [JsonPropertyName("items")]
    public List<TrashItem> Items { get; init; } = [];
}

public sealed class TrashItem
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; init; } = string.Empty;
}
