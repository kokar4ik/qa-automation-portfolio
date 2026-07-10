using System.Text.Json.Serialization;

namespace YandexDiskApi.Api.Models;

public sealed class UploadLinkResponse
{
    [JsonPropertyName("href")]
    public string Href { get; init; } = string.Empty;

    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;
}