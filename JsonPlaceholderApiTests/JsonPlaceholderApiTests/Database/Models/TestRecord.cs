using JsonPlaceholderApiTests.Database.Mapping;

namespace JsonPlaceholderApiTests.Database.Models;

[Table("test")]
public class TestRecord
{
    [Column("id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("status_id")]
    public int? StatusId { get; set; }

    [Column("method_name")]
    public string MethodName { get; set; } = string.Empty;

    [Column("project_id")]
    public long ProjectId { get; set; }

    [Column("session_id")]
    public long SessionId { get; set; }

    [Column("start_time")]
    public DateTime? StartTime { get; set; }

    [Column("end_time")]
    public DateTime? EndTime { get; set; }

    [Column("env")]
    public string Env { get; set; } = string.Empty;

    [Column("browser")]
    public string Browser { get; set; } = string.Empty;

    [Column("author_id")]
    public long? AuthorId { get; set; }
}
