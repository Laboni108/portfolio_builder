using Dapper;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Data;

public class CodingHighlightRepository
{
    private readonly DbConnectionFactory _factory;

    public CodingHighlightRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<CodingHighlight>> GetAllAsync()
    {
        const string sql = @"SELECT Id, Icon, Title, DisplayOrder, CreatedAt
                             FROM dbo.CodingHighlights
                             ORDER BY DisplayOrder, Id";

        using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<CodingHighlight>(sql);
    }

    public async Task<CodingHighlight?> GetByIdAsync(int id)
    {
        const string sql = @"SELECT Id, Icon, Title, DisplayOrder, CreatedAt
                             FROM dbo.CodingHighlights
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CodingHighlight>(sql, new { Id = id });
    }

    public async Task<int> AddAsync(CodingHighlight highlight)
    {
        const string sql = @"INSERT INTO dbo.CodingHighlights (Icon, Title, DisplayOrder)
                             OUTPUT INSERTED.Id
                             VALUES (@Icon, @Title, @DisplayOrder)";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, highlight);
    }

    public async Task<bool> UpdateAsync(CodingHighlight highlight)
    {
        const string sql = @"UPDATE dbo.CodingHighlights
                             SET Icon = @Icon,
                                 Title = @Title,
                                 DisplayOrder = @DisplayOrder
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, highlight);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM dbo.CodingHighlights WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}