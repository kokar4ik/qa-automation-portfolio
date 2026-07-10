namespace JsonPlaceholderApiTests.Database.Mapping;

[AttributeUsage(AttributeTargets.Class)]
public sealed class TableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}