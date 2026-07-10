using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Database.Repositories;

public class StatusRepository : RepositoryBase<StatusRecord, int>
{
    public StatusRepository(DatabaseConnection databaseConnection)
        : base(databaseConnection)
    {
    }

    public override Task<long> CreateAsync(StatusRecord record, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("Status table is read-only in tests.");

    public override Task UpdateAsync(StatusRecord record, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("Status table is read-only in tests.");

    public Task<StatusRecord?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        GetByPropertyAsync(nameof(StatusRecord.Name), name, cancellationToken);
}