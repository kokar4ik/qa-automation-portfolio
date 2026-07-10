using JsonPlaceholderApiTests.Database.Models;

namespace JsonPlaceholderApiTests.Database.Repositories;

public class ProjectRepository : RepositoryBase<ProjectRecord, long>
{
    public ProjectRepository(DatabaseConnection databaseConnection)
        : base(databaseConnection)
    {
    }

    public Task<ProjectRecord?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        GetByPropertyAsync(nameof(ProjectRecord.Name), name, cancellationToken);

    public async Task<ProjectRecord> GetOrCreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var existingProject = await GetByNameAsync(name, cancellationToken);
        if (existingProject is not null)
        {
            return existingProject;
        }

        var projectId = await CreateAsync(new ProjectRecord { Name = name }, cancellationToken);
        return (await GetByIdAsync(projectId, cancellationToken))!;
    }
}