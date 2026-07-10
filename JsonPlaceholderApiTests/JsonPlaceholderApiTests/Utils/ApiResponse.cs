using System.Net;

namespace JsonPlaceholderApiTests.Utils;

public class ApiResponse<T>
{
    public HttpStatusCode StatusCode { get; init; }
    public T? Body { get; init; }
    public string RawContent { get; init; } = string.Empty;
}
