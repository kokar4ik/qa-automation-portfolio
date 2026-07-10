namespace JsonPlaceholderApiTests.Database.Repositories;

public sealed class ColumnBinding<TRecord>
{
    public required string ColumnName { get; init; }
    public required Func<TRecord, object?> GetValue { get; init; }

    public string ParameterName => SqlTemplates.ToParameterName(ColumnName);
}