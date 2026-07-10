namespace JsonPlaceholderApiTests.Configuration;

public class TestDataSettings
{
    public required int ExistingPostId { get; set; }
    public required int ExistingPostUserId { get; set; }
    public required int MissingPostId { get; set; }
    public required string MissingPostEmptyBody { get; set; }
    public required int ExistingUserId { get; set; }
    public required int CreatePostUserId { get; set; }
    public required string ExpectedUserFile { get; set; }
}