using JsonPlaceholderApiTests.Database.Mapping;

namespace JsonPlaceholderApiTests.Database.Models;

[Table("session")]
public class SessionRecord
{
    [Column("id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [Column("session_key")]
    public string SessionKey { get; set; } = string.Empty;

    [Column("created_time")]
    public DateTime CreatedTime { get; set; }

    [Column("build_number")]
    public long BuildNumber { get; set; }
}
