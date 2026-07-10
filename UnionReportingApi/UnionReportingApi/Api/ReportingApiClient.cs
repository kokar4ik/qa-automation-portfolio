using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using UnionReportingApi.Api.Models;
using UnionReportingApi.Config;
using UnionReportingApi.Constants;
using UnionReportingApi.TestData;

namespace UnionReportingApi.Api;

public sealed class ReportingApiClient : IDisposable
{
    private const int FirstRetryAttempt = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly RestClient client;
    private readonly int apiFormatRetryCount;

    public ReportingApiClient(string apiBaseUrl, string login, string password, int apiFormatRetryCount)
    {
        if (string.IsNullOrWhiteSpace(apiBaseUrl))
        {
            throw new ArgumentException("Требуется базовый URL API.", nameof(apiBaseUrl));
        }

        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Требуется логин.", nameof(login));
        }

        if (apiFormatRetryCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(apiFormatRetryCount));
        }

        this.apiFormatRetryCount = apiFormatRetryCount;

        var options = new RestClientOptions(NormalizeBaseUrl(apiBaseUrl))
        {
            Authenticator = new HttpBasicAuthenticator(login, password),
        };

        client = new RestClient(options);
    }

    public static ReportingApiClient FromConfiguration()
    {
        var config = TestConfiguration.Instance;
        var testData = TestDataProvider.Instance.Data;
        return new ReportingApiClient(
            config.ApiBaseUrl,
            config.Login,
            config.Password,
            testData.ApiFormatRetryCount);
    }

    public RestResponse GenerateToken(int variantId)
    {
        var request = CreateRequest(ApiConstants.Endpoints.TokenGet, Method.Post);
        request.AddQueryParameter(ApiConstants.QueryParameters.Variant, variantId.ToString());
        return client.Execute(request);
    }

    private RestResponse GetTestsJson(long projectId)
    {
        var request = CreateRequest(ApiConstants.Endpoints.TestGetJson, Method.Post);
        request.AddQueryParameter(ApiConstants.QueryParameters.ProjectId, projectId.ToString());
        return client.Execute(request);
    }

    public bool TryGetProjectTestsFromJson(
        long projectId,
        out IReadOnlyList<ProjectTestItem> tests,
        out RestResponse lastResponse)
    {
        return TryGetProjectTestsWithRetry(
            projectId,
            GetTestsJson,
            IsJsonArray,
            ParseProjectTestsFromJson,
            out tests,
            out lastResponse);
    }

    public RestResponse CreateTest(CreateTestRequest requestModel)
    {
        var request = CreateRequest(ApiConstants.Endpoints.TestPut, Method.Post);
        request.AddParameter(ApiConstants.FormParameters.SessionId, requestModel.SessionId);
        request.AddParameter(ApiConstants.FormParameters.ProjectName, requestModel.ProjectName);
        request.AddParameter(ApiConstants.FormParameters.TestName, requestModel.TestName);
        request.AddParameter(ApiConstants.FormParameters.MethodName, requestModel.MethodName);
        request.AddParameter(ApiConstants.FormParameters.Environment, requestModel.Environment);
        request.AddParameter(ApiConstants.FormParameters.Browser, requestModel.Browser);

        return client.Execute(request);
    }

    public static bool TryParseTestId(RestResponse response, out long testId) =>
        long.TryParse(response.Content?.Trim(), out testId);

    public RestResponse AddLog(long testId, string content, bool isException = false)
    {
        var request = CreateRequest(ApiConstants.Endpoints.TestPutLog, Method.Post);
        request.AddParameter(ApiConstants.FormParameters.TestId, testId);
        request.AddParameter(ApiConstants.FormParameters.Content, content);
        request.AddParameter(
            ApiConstants.FormParameters.IsException,
            isException ? ApiConstants.Protocol.BooleanTrue : ApiConstants.Protocol.BooleanFalse);
        return client.Execute(request);
    }

    public RestResponse AddAttachment(long testId, AttachmentContent attachment)
    {
        var request = CreateRequest(ApiConstants.Endpoints.TestPutAttachment, Method.Post);
        request.AddQueryParameter(ApiConstants.QueryParameters.TestId, testId.ToString());
        request.AddJsonBody(attachment);
        return client.Execute(request);
    }

    public void Dispose() => client.Dispose();

    private static IReadOnlyList<ProjectTestItem> ParseProjectTestsFromJson(string json)
    {
        var items = JsonSerializer.Deserialize<List<ProjectTestItem>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Не удалось десериализовать JSON со списком тестов.");

        return items;
    }

    private bool TryGetProjectTestsWithRetry(
        long projectId,
        Func<long, RestResponse> requestFactory,
        Func<string, bool> responseValidator,
        Func<string, IReadOnlyList<ProjectTestItem>> parser,
        out IReadOnlyList<ProjectTestItem> tests,
        out RestResponse lastResponse)
    {
        tests = Array.Empty<ProjectTestItem>();
        lastResponse = new RestResponse();

        for (var attempt = FirstRetryAttempt; attempt <= apiFormatRetryCount; attempt++)
        {
            lastResponse = requestFactory(projectId);

            var content = lastResponse.Content ?? string.Empty;
            if (lastResponse.IsSuccessful && responseValidator(content))
            {
                tests = parser(content);
                return true;
            }
        }

        return false;
    }

    private static RestRequest CreateRequest(string resource, Method method) =>
        new(resource, method);

    private static string NormalizeBaseUrl(string apiBaseUrl) =>
        apiBaseUrl.EndsWith(ApiConstants.Protocol.UrlTrailingSlash)
            ? apiBaseUrl.TrimEnd(ApiConstants.Protocol.UrlTrailingSlash)
            : apiBaseUrl;

    private static bool IsJsonArray(string content)
    {
        var trimmed = content.TrimStart();
        return trimmed.StartsWith(ApiConstants.Protocol.JsonArrayOpen)
            && trimmed.EndsWith(ApiConstants.Protocol.JsonArrayClose);
    }
}
