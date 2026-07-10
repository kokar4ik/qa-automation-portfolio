using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Database.Repositories;

public class SessionRepository : RepositoryBase<SessionRecord, long>
{
    public SessionRepository(DatabaseConnection databaseConnection)
        : base(databaseConnection)
    {
    }
}