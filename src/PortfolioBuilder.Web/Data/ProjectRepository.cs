using Dapper;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Data;

public class ProjectRepository
{
    private readonly DbConnectionFactory _factory;

    public ProjectRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        const string sql = @"SELECT Id, Title, Description, TechStack, GithubUrl, CreatedAt
                             FROM dbo.Projects
                             ORDER BY CreatedAt DESC";

        using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<Project>(sql);
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        const string sql = @"SELECT Id, Title, Description, TechStack, GithubUrl, CreatedAt
                             FROM dbo.Projects
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Project>(sql, new { Id = id });
    }

        public async Task<int> AddAsync(Project project)
    {
        const string sql = @"INSERT INTO dbo.Projects (Title, Description, TechStack, GithubUrl)
                             OUTPUT INSERTED.Id
                             VALUES (@Title, @Description, @TechStack, @GithubUrl)";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM dbo.Projects WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}