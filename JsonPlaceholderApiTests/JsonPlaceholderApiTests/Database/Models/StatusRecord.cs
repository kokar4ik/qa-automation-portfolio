using JsonPlaceholderApiTests.Database.Mapping;

namespace JsonPlaceholderApiTests.Database.Models;

[Table("status")]
public class StatusRecord
{
    [Column("id", IsPrimaryKey = true)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;
}
