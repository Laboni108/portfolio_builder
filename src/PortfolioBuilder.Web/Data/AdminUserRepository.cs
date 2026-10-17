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

        public async Task SetRememberTokenAsync(int id, string tokenHash, DateTime expiresAtUtc)
    {
        const string sql = @"UPDATE dbo.AdminUsers
                             SET RememberTokenHash = @TokenHash,
                                 RememberTokenExpiresAt = @ExpiresAt
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id, TokenHash = tokenHash, ExpiresAt = expiresAtUtc });
    }

    public async Task<AdminUser?> GetByRememberTokenHashAsync(string tokenHash)
    {
        const string sql = @"SELECT Id, Username, PasswordHash, CreatedAt
                             FROM dbo.AdminUsers
                             WHERE RememberTokenHash = @TokenHash
                               AND RememberTokenExpiresAt > SYSUTCDATETIME()";

        using var connection = _factory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AdminUser>(sql, new { TokenHash = tokenHash });
    }

    public async Task ClearRememberTokenAsync(int id)
    {
        const string sql = @"UPDATE dbo.AdminUsers
                             SET RememberTokenHash = NULL,
                                 RememberTokenExpiresAt = NULL
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}