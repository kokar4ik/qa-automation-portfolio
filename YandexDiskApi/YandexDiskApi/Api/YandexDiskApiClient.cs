using RestSharp;
using YandexDiskApi.Api.Models;
using YandexDiskApi.Config;
using YandexDiskApi.Constants;
using YandexDiskApi.Utils;

namespace YandexDiskApi.Api;

public sealed class YandexDiskApiClient : IDisposable
{
    private readonly RestClient client;
    private const string RootPath = "/";

    public YandexDiskApiClient(string oauthToken, string apiBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(oauthToken))
        {
            throw new ArgumentException("Требуется OAuth-токен.", nameof(oauthToken));
        }

        if (string.IsNullOrWhiteSpace(apiBaseUrl))
        {
            throw new ArgumentException("Требуется базовый URL API.", nameof(apiBaseUrl));
        }

        client = new RestClient(new RestClientOptions(apiBaseUrl));
        client.AddDefaultHeader(KnownHeaders.Authorization, $"OAuth {oauthToken}");
    }

    public static YandexDiskApiClient FromConfiguration() =>
        new(TestConfiguration.Instance.OAuthToken, TestConfiguration.Instance.ApiBaseUrl);

    public RestResponse GetDisk() =>
        client.Execute(CreateRequest(ApiEndpoints.Disk, Method.Get));

    public RestResponse<UploadLinkResponse> GetUploadLink(string remotePath, bool overwrite)
    {
        var request = CreateRequest(ApiEndpoints.DiskResourcesUpload, Method.Get);
        request.AddQueryParameter(ApiQueryParameters.Path, DiskPathUtils.Normalize(remotePath));
        request.AddQueryParameter(ApiQueryParameters.Overwrite, overwrite.ToString().ToLowerInvariant());
        return client.Execute<UploadLinkResponse>(request);
    }

    public RestResponse<UploadLinkResponse> GetDownloadLink(string remotePath)
    {
        var request = CreateRequest($"{ApiEndpoints.DiskResources}/download", Method.Get);
        request.AddQueryParameter(ApiQueryParameters.Path, DiskPathUtils.Normalize(remotePath));
        return client.Execute<UploadLinkResponse>(request);
    }

    public RestResponse UploadToHref(string uploadHref, byte[] content, string contentType)
    {
        var request = new RestRequest(uploadHref, Method.Put);
        request.AddBody(content, contentType);
        return client.Execute(request);
    }

    public RestResponse MoveResource(string fromPath, string toPath)
    {
        var request = CreateRequest(ApiEndpoints.DiskResourcesMove, Method.Post);
        request.AddQueryParameter(ApiQueryParameters.From, DiskPathUtils.Normalize(fromPath));
        request.AddQueryParameter(ApiQueryParameters.Path, DiskPathUtils.Normalize(toPath));
        return client.Execute(request);
    }

    public RestResponse<TrashResourcesResponse> GetTrashResources(int limit = 100)
    {
        var request = CreateRequest(ApiEndpoints.DiskTrashResources, Method.Get);
        request.AddQueryParameter(ApiQueryParameters.Path, RootPath);
        request.AddQueryParameter(ApiQueryParameters.Limit, limit.ToString());
        return client.Execute<TrashResourcesResponse>(request);
    }

    public RestResponse RestoreFromTrash(string trashPath)
    {
        var request = CreateRequest(ApiEndpoints.DiskTrashResourcesRestore, Method.Put);
        request.AddQueryParameter(ApiQueryParameters.Path, trashPath);
        return client.Execute(request);
    }

    public RestResponse DeletePermanently(string remotePath)
    {
        var request = CreateRequest(ApiEndpoints.DiskResources, Method.Delete);
        request.AddQueryParameter(ApiQueryParameters.Path, DiskPathUtils.Normalize(remotePath));
        request.AddQueryParameter(ApiQueryParameters.Permanently, true.ToString().ToLowerInvariant());
        return client.Execute(request);
    }

    public RestResponse DownloadFromHref(string downloadHref)
    {
        var request = new RestRequest(downloadHref, Method.Get);
        return client.Execute(request);
    }

    public void Dispose() => client.Dispose();

    private static RestRequest CreateRequest(string resource, Method method) =>
        new(resource, method);
}
