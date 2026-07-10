using JsonPlaceholderApiTests.Database.Mapping;
using MySqlConnector;

namespace JsonPlaceholderApiTests.Database.Repositories;

public abstract class RepositoryBase<TRecord, TId> where TRecord : class, new()
{
    private readonly DatabaseConnection _databaseConnection;

    protected RepositoryBase(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    protected DatabaseConnection DatabaseConnection => _databaseConnection;

    protected virtual string TableName => EntityMapper<TRecord>.TableName;

    protected string SelectColumns => EntityMapper<TRecord>.SelectColumns;

    protected virtual IReadOnlyList<ColumnBinding<TRecord>> InsertColumns =>
        EntityMapper<TRecord>.InsertColumns;

    protected virtual IReadOnlyList<ColumnBinding<TRecord>> UpdateColumns =>
        EntityMapper<TRecord>.UpdateColumns;

    public virtual async Task<TRecord?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var connection = _databaseConnection.Connection;
        await using var command = new MySqlCommand(
            SqlTemplates.SelectWhere(SelectColumns, TableName, "id = @id"),
            connection);
        command.Parameters.AddWithValue("@id", id);

        return await ReadSingleAsync(command, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var connection = _databaseConnection.Connection;
        await using var command = new MySqlCommand(
            SqlTemplates.SelectAllOrdered(SelectColumns, TableName),
            connection);

        return await ReadAllAsync(command, cancellationToken);
    }

    protected Task<TRecord?> GetByPropertyAsync(
        string propertyName,
        object value,
        CancellationToken cancellationToken = default) =>
        GetByColumnAsync(EntityMapper<TRecord>.GetColumnName(propertyName), value, cancellationToken);

    protected async Task<TRecord?> GetByColumnAsync(
        string columnName,
        object value,
        CancellationToken cancellationToken = default)
    {
        var parameterName = SqlTemplates.ToParameterName(columnName);
        var connection = _databaseConnection.Connection;
        await using var command = new MySqlCommand(
            SqlTemplates.SelectWhere(SelectColumns, TableName, $"{columnName} = {parameterName}"),
            connection);
        command.Parameters.AddWithValue(parameterName, value);

        return await ReadSingleAsync(command, cancellationToken);
    }

    public virtual async Task<long> CreateAsync(TRecord record, CancellationToken cancellationToken = default)
    {
        var columns = InsertColumns.Select(binding => binding.ColumnName).ToArray();
        var connection = _databaseConnection.Connection;
        await using var command = new MySqlCommand(
            SqlTemplates.Insert(TableName, columns),
            connection);
        AddBindings(command, record, InsertColumns);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result);
    }

    public virtual async Task UpdateAsync(TRecord record, CancellationToken cancellationToken = default)
    {
        var columns = UpdateColumns.Select(binding => binding.ColumnName).ToArray();
        var connection = _databaseConnection.Connection;
        await using var command = new MySqlCommand(
            SqlTemplates.Update(TableName, columns),
            connection);
        command.Parameters.AddWithValue("@id", EntityMapper<TRecord>.GetId<TId>(record));
        AddBindings(command, record, UpdateColumns);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var connection = _databaseConnection.Connection;
        await using var command = new MySqlCommand(
            SqlTemplates.Delete(TableName),
            connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    protected async Task<IReadOnlyList<TRecord>> ReadAllAsync(
        MySqlCommand command,
        CancellationToken cancellationToken)
    {
        var records = new List<TRecord>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            records.Add(EntityMapper<TRecord>.Map(reader));
        }

        return records;
    }

    protected async Task<TRecord?> ReadSingleAsync(
        MySqlCommand command,
        CancellationToken cancellationToken)
    {
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? EntityMapper<TRecord>.Map(reader) : null;
    }

    private static void AddBindings(
        MySqlCommand command,
        TRecord record,
        IReadOnlyList<ColumnBinding<TRecord>> bindings)
    {
        foreach (var binding in bindings)
        {
            command.Parameters.AddWithValue(binding.ParameterName, binding.GetValue(record) ?? DBNull.Value);
        }
    }
}