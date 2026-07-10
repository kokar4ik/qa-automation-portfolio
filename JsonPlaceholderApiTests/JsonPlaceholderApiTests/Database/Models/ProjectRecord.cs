using JsonPlaceholderApiTests.Database.Mapping;

namespace JsonPlaceholderApiTests.Database.Models;

[Table("project")]
public class ProjectRecord
{
    [Column("id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;
}
