namespace JsonPlaceholderApiTests.Configuration;

public class DatabaseTestDataSettings
{
    public required string ProjectName { get; set; }
    public required string AuthorName { get; set; }
    public required string AuthorLogin { get; set; }
    public required string AuthorEmail { get; set; }
    public int RepeatingDigitTestsLimit { get; set; }
    public int CopySourceTestsLimit { get; set; }
    public int CreateTestDurationSeconds { get; set; }
    public int UpdateTestDurationSeconds { get; set; }
}