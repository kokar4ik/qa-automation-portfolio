using System.Text.Json.Serialization;

namespace UnionReportingApi.Api.Models;

public sealed class ProjectTestItem
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("method")]
    public required string Method { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("startTime")]
    public string? StartTime { get; init; }
}
