namespace UnionReportingApi.Api.Models;

public sealed class CreateTestRequest
{
    public required string SessionId { get; init; }

    public required string ProjectName { get; init; }

    public required string TestName { get; init; }

    public required string MethodName { get; init; }

    public required string Environment { get; init; }

    public required string Browser { get; init; }
}
