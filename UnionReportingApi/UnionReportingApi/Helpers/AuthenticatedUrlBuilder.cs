namespace UnionReportingApi.Helpers;

public static class AuthenticatedUrlBuilder
{
    public static string Build(string baseUrl, string relativePath, string login, string password)
    {
        var baseUri = new Uri(baseUrl, UriKind.Absolute);
        var targetUri = new Uri(baseUri, relativePath);
        var builder = new UriBuilder(targetUri)
        {
            UserName = login,
            Password = password,
        };

        return builder.Uri.ToString();
    }
}
