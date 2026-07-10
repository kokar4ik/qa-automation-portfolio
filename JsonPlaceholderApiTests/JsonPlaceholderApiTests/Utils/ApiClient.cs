using System.Net.Mime;
using System.Text;
using System.Text.Json;
using JsonPlaceholderApiTests.Configuration;
using JsonPlaceholderApiTests.Constants;

namespace JsonPlaceholderApiTests.Utils;

public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public ApiClient(ApiSettings settings)
    {
        var uriBuilder = new UriBuilder(settings.BaseUrl);

        if (!uriBuilder.Path.EndsWith(ConfigurationConstants.PathSeparator))
        {
            uriBuilder.Path = string.Concat(
                uriBuilder.Path.TrimEnd(ConfigurationConstants.PathSeparator),
                ConfigurationConstants.PathSeparator);
        }

        _httpClient = new HttpClient
        {
            BaseAddress = uriBuilder.Uri,
            Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds)
        };

        TestLogger.LogInfo($"ApiClient инициализирован. BaseUrl: {uriBuilder.Uri}");
    }

    protected async Task<ApiResponse<T>> GetAsync<T>(string path)
    {
        var normalizedPath = NormalizePath(path);
        TestLogger.LogInfo($"GET {normalizedPath}");

        var response = await _httpClient.GetAsync(normalizedPath);
        var content = await response.Content.ReadAsStringAsync();

        TestLogger.LogInfo($"GET {normalizedPath} -> {(int)response.StatusCode} {response.StatusCode}");

        return new ApiResponse<T>
        {
            StatusCode = response.StatusCode,
            RawContent = content,
            Body = DeserializeBody<T>(content)
        };
    }

    protected async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string path, TRequest body)
    {
        var normalizedPath = NormalizePath(path);
        var json = JsonSerializer.Serialize(body, JsonSerializerDefaults.Options);
        TestLogger.LogInfo($"POST {normalizedPath}. Тело запроса: {json}");

        using var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.PostAsync(normalizedPath, httpContent);
        var content = await response.Content.ReadAsStringAsync();

        TestLogger.LogInfo($"POST {normalizedPath} -> {(int)response.StatusCode} {response.StatusCode}");

        return new ApiResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            RawContent = content,
            Body = DeserializeBody<TResponse>(content)
        };
    }

    protected static string BuildPath(string endpoint, string? resourceId = null)
    {
        var normalizedEndpoint = endpoint.Trim(ConfigurationConstants.PathSeparator);

        if (string.IsNullOrEmpty(resourceId))
        {
            return normalizedEndpoint;
        }

        return Path.Combine(normalizedEndpoint, resourceId.Trim(ConfigurationConstants.PathSeparator))
            .Replace('\\', ConfigurationConstants.PathSeparator);
    }

    private static string NormalizePath(string path) =>
        path.TrimStart(ConfigurationConstants.PathSeparator);

    private static T? DeserializeBody<T>(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(content, JsonSerializerDefaults.Options);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        TestLogger.LogInfo("ApiClient освобождён.");
    }
}
