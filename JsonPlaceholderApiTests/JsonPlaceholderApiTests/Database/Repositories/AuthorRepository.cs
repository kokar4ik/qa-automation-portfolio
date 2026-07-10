using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Database.Repositories;

public class AuthorRepository : RepositoryBase<AuthorRecord, long>
{
    public AuthorRepository(DatabaseConnection databaseConnection)
        : base(databaseConnection)
    {
    }

    public Task<AuthorRecord?> GetByLoginAsync(string login, CancellationToken cancellationToken = default) =>
        GetByPropertyAsync(nameof(AuthorRecord.Login), login, cancellationToken);

    public async Task<AuthorRecord> GetOrCreateAsync(
        string name,
        string login,
        string email,
        CancellationToken cancellationToken = default)
    {
        var existingAuthor = await GetByLoginAsync(login, cancellationToken);
        if (existingAuthor is not null)
        {
            return existingAuthor;
        }

        var authorId = await CreateAsync(new AuthorRecord
        {
            Name = name,
            Login = login,
            Email = email
        }, cancellationToken);

        return (await GetByIdAsync(authorId, cancellationToken))!;
    }
}