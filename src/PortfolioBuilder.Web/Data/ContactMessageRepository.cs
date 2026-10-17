using Dapper;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Data;

public class ContactMessageRepository
{
    private readonly DbConnectionFactory _factory;

    public ContactMessageRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<int> AddAsync(ContactMessage message)
    {
        const string sql = @"INSERT INTO dbo.ContactMessages (Name, Email, Subject, Message)
                             OUTPUT INSERTED.Id
                             VALUES (@Name, @Email, @Subject, @Message)";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, message);
    }

    public async Task<IEnumerable<ContactMessage>> GetAllAsync()
    {
        const string sql = @"SELECT Id, Name, Email, Subject, Message, IsRead, CreatedAt
                             FROM dbo.ContactMessages
                             ORDER BY CreatedAt DESC";

        using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<ContactMessage>(sql);
    }

    public async Task<int> CountUnreadAsync()
    {
        const string sql = "SELECT COUNT(*) FROM dbo.ContactMessages WHERE IsRead = 0";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        const string sql = "UPDATE dbo.ContactMessages SET IsRead = 1 WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM dbo.ContactMessages WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

        public async Task<ContactMessage?> GetByIdAsync(int id)
    {
        const string sql = @"SELECT Id, Name, Email, Subject, Message, IsRead, CreatedAt
                             FROM dbo.ContactMessages
                             WHERE Id = @Id";

        using var connection = _factory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ContactMessage>(sql, new { Id = id });
    }
}