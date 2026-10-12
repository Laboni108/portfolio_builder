using Dapper;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Data;

public class AdminUserRepository
{
    private readonly DbConnectionFactory _factory;

    public AdminUserRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username)
    {
        const string sql = @"SELECT Id, Username, PasswordHash, CreatedAt
                             FROM dbo.AdminUsers
                             WHERE Username = @Username";

        using var connection = _factory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AdminUser>(sql, new { Username = username });
    }

    public async Task<int> CountAsync()
    {
        const string sql = "SELECT COUNT(*) FROM dbo.AdminUsers";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<int> AddAsync(AdminUser user)
    {
        const string sql = @"INSERT INTO dbo.AdminUsers (Username, PasswordHash)
                             OUTPUT INSERTED.Id
                             VALUES (@Username, @PasswordHash)";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}