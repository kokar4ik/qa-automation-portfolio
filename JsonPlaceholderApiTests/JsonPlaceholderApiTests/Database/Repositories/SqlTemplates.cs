namespace JsonPlaceholderApiTests.Database.Repositories;

public static class SqlTemplates
{
    public static string Select(string columns, string table) =>
        $"SELECT {columns} FROM {table}";

    public static string SelectWhere(string columns, string table, string whereClause) =>
        $"{Select(columns, table)} WHERE {whereClause}";

    public static string SelectAllOrdered(string columns, string table, string orderBy = "id") =>
        $"{Select(columns, table)} ORDER BY {orderBy}";

    public static string Insert(string table, IReadOnlyList<string> columns)
    {
        var columnList = string.Join(", ", columns);
        var parameters = string.Join(", ", columns.Select(ToParameterName));
        return $"INSERT INTO {table} ({columnList}) VALUES ({parameters}); SELECT LAST_INSERT_ID();";
    }

    public static string Update(string table, IReadOnlyList<string> columns)
    {
        var assignments = string.Join(", ", columns.Select(column => $"{column} = {ToParameterName(column)}"));
        return $"UPDATE {table} SET {assignments} WHERE id = @id";
    }

    public static string Delete(string table) =>
        $"DELETE FROM {table} WHERE id = @id";

    public static string ToParameterName(string columnName) =>
        $"@{ToCamelCase(columnName)}";

    private static string ToCamelCase(string snakeCase)
    {
        var parts = snakeCase.Split('_');
        return parts[0] + string.Concat(parts.Skip(1).Select(part =>
            char.ToUpper(part[0]) + part[1..]));
    }
}