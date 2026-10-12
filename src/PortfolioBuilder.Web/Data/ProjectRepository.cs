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
}