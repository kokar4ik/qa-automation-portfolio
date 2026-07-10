using JsonPlaceholderApiTests.Database.Mapping;

namespace JsonPlaceholderApiTests.Database.Models;

[Table("author")]
public class AuthorRecord
{
    [Column("id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("login")]
    public string Login { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;
}
