namespace JsonPlaceholderApiTests.Configuration;

public class ApiSettings
{
    public required string BaseUrl { get; set; }
    public required string PostsEndpoint { get; set; }
    public required string UsersEndpoint { get; set; }
    public required int TimeoutSeconds { get; set; }
}