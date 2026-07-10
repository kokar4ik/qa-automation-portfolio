using System.Text.Json.Serialization;

namespace UnionReportingApi.Api.Models;

public sealed class AttachmentContent
{
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    [JsonPropertyName("contentType")]
    public required string ContentType { get; init; }
}
