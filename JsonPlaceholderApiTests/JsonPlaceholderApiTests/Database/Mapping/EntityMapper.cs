using System.Reflection;
using JsonPlaceholderApiTests.Database.Repositories;
using MySqlConnector;

namespace JsonPlaceholderApiTests.Database.Mapping;

public static class EntityMapper<TRecord> where TRecord : class, new()
{
    private static readonly EntityMetadata Metadata = EntityMetadata.Create<TRecord>();

    public static string TableName => Metadata.TableName;

    public static string SelectColumns => Metadata.SelectColumns;

    public static IReadOnlyList<ColumnBinding<TRecord>> InsertColumns => Metadata.InsertColumns;

    public static IReadOnlyList<ColumnBinding<TRecord>> UpdateColumns => Metadata.UpdateColumns;

    public static TId GetId<TId>(TRecord record) => (TId)Metadata.PrimaryKey.GetValue(record)!;

    public static string GetColumnName(string propertyName) => Metadata.GetColumnName(propertyName);

    public static TRecord Map(MySqlDataReader reader)
    {
        var record = new TRecord();

        foreach (var column in Metadata.Columns)
        {
            var ordinal = reader.GetOrdinal(column.ColumnName);
            var value = reader.IsDBNull(ordinal)
                ? GetNullValue(column.Property.PropertyType)
                : ReadValue(reader, ordinal, column.Property.PropertyType);
            column.Property.SetValue(record, value);
        }

        return record;
    }

    private static object? GetNullValue(Type propertyType)
    {
        if (propertyType == typeof(string))
        {
            return string.Empty;
        }

        if (Nullable.GetUnderlyingType(propertyType) is not null)
        {
            return null;
        }

        return null;
    }

    private static object ReadValue(MySqlDataReader reader, int ordinal, Type propertyType)
    {
        var valueType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (valueType == typeof(long))
        {
            return reader.GetInt64(ordinal);
        }

        if (valueType == typeof(int))
        {
            return reader.GetInt32(ordinal);
        }

        if (valueType == typeof(string))
        {
            return reader.GetString(ordinal);
        }

        if (valueType == typeof(DateTime))
        {
            return reader.GetDateTime(ordinal);
        }

        throw new NotSupportedException($"Column type '{valueType.Name}' is not supported.");
    }

    private sealed class EntityMetadata
    {
        private readonly Dictionary<string, string> _propertyToColumn;

        public string TableName { get; }
        public string SelectColumns { get; }
        public IReadOnlyList<MappedColumn> Columns { get; }
        public PropertyInfo PrimaryKey { get; }
        public IReadOnlyList<ColumnBinding<TRecord>> InsertColumns { get; }
        public IReadOnlyList<ColumnBinding<TRecord>> UpdateColumns { get; }

        private EntityMetadata(
            string tableName,
            IReadOnlyList<MappedColumn> columns,
            PropertyInfo primaryKey,
            IReadOnlyList<ColumnBinding<TRecord>> dataBindings)
        {
            TableName = tableName;
            Columns = columns;
            PrimaryKey = primaryKey;
            SelectColumns = string.Join(", ", columns.Select(column => column.ColumnName));
            InsertColumns = dataBindings;
            UpdateColumns = dataBindings;
            _propertyToColumn = columns.ToDictionary(
                column => column.Property.Name,
                column => column.ColumnName);
        }

        public static EntityMetadata Create<T>() where T : class, new()
        {
            var type = typeof(T);
            var tableAttribute = type.GetCustomAttribute<TableAttribute>()
                ?? throw new InvalidOperationException($"Type '{type.Name}' must have [Table] attribute.");

            var columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(property =>
                {
                    var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                    return columnAttribute is null
                        ? null
                        : new MappedColumn
                        {
                            Property = property,
                            ColumnName = columnAttribute.Name,
                            IsPrimaryKey = columnAttribute.IsPrimaryKey
                        };
                })
                .Where(column => column is not null)
                .Cast<MappedColumn>()
                .ToArray();

            if (columns.Length == 0)
            {
                throw new InvalidOperationException($"Type '{type.Name}' must have at least one [Column] attribute.");
            }

            var primaryKeyColumn = columns.SingleOrDefault(column => column.IsPrimaryKey)
                ?? columns.SingleOrDefault(column => column.Property.Name == "Id")
                ?? throw new InvalidOperationException($"Type '{type.Name}' must define a primary key column.");

            var dataBindings = columns
                .Where(column => column.Property != primaryKeyColumn.Property)
                .Select(column => new ColumnBinding<TRecord>
                {
                    ColumnName = column.ColumnName,
                    GetValue = record => column.Property.GetValue(record)
                })
                .ToArray();

            return new EntityMetadata(tableAttribute.Name, columns, primaryKeyColumn.Property, dataBindings);
        }

        public string GetColumnName(string propertyName)
        {
            if (!_propertyToColumn.TryGetValue(propertyName, out var columnName))
            {
                throw new ArgumentException($"Property '{propertyName}' is not mapped with [Column].", nameof(propertyName));
            }

            return columnName;
        }
    }

    private sealed class MappedColumn
    {
        public required PropertyInfo Property { get; init; }
        public required string ColumnName { get; init; }
        public bool IsPrimaryKey { get; init; }
    }
}
