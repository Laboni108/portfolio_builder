using Dapper;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Data;

public class SkillCategoryRepository
{
    private readonly DbConnectionFactory _factory;

    public SkillCategoryRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<SkillCategory>> GetAllAsync()
    {
        const string sql = @"SELECT Id, Name, IconClass, Items, DisplayOrder, CreatedAt
                             FROM dbo.SkillCategories
                             ORDER BY DisplayOrder, Id";

        using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<SkillCategory>(sql);
    }

    public async Task<SkillCategory?> GetByIdAsync(int id)
    {
        const string sql = @"SELECT Id, Name, IconClass, Items, DisplayOrder, CreatedAt
                             FROM dbo.SkillCategories
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SkillCategory>(sql, new { Id = id });
    }

    public async Task<int> AddAsync(SkillCategory category)
    {
        const string sql = @"INSERT INTO dbo.SkillCategories (Name, IconClass, Items, DisplayOrder)
                             OUTPUT INSERTED.Id
                             VALUES (@Name, @IconClass, @Items, @DisplayOrder)";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, category);
    }

    public async Task<bool> UpdateAsync(SkillCategory category)
    {
        const string sql = @"UPDATE dbo.SkillCategories
                             SET Name = @Name,
                                 IconClass = @IconClass,
                                 Items = @Items,
                                 DisplayOrder = @DisplayOrder
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, category);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM dbo.SkillCategories WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}