namespace JsonPlaceholderApiTests.Database.Mapping;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnAttribute(string name) : Attribute
{
    public string Name { get; } = name;

    public bool IsPrimaryKey { get; init; }
}