using JsonPlaceholderApiTests.Database.Models;
using MySqlConnector;

namespace JsonPlaceholderApiTests.Database.Repositories;

public class TestRepository : RepositoryBase<TestRecord, long>
{
    public TestRepository(DatabaseConnection databaseConnection) 
        : base(databaseConnection)
    {
    }

    public async Task<IReadOnlyList<TestRecord>> GetByRepeatingDigitIdsAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        var connection = DatabaseConnection.Connection;
        await using var command = new MySqlCommand(
            $"""
            {SqlTemplates.SelectWhere(
                SelectColumns,
                TableName,
                "CAST(id AS CHAR) REGEXP '00|11|22|33|44|55|66|77|88|99'")}
            ORDER BY id
            LIMIT @limit
            """,
            connection);
        command.Parameters.AddWithValue("@limit", limit);

        return await ReadAllAsync(command, cancellationToken);
    }

    public async Task<TestRecord> CopyAsync(
        TestRecord source,
        long projectId,
        long authorId,
        long sessionId,
        CancellationToken cancellationToken = default)
    {
        var copy = new TestRecord
        {
            Name = source.Name,
            StatusId = source.StatusId,
            MethodName = source.MethodName,
            ProjectId = projectId,
            SessionId = sessionId,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            Env = source.Env,
            Browser = source.Browser,
            AuthorId = authorId
        };

        copy.Id = await CreateAsync(copy, cancellationToken);
        return copy;
    }
}