using System.Data;
using Microsoft.Data.SqlClient;

namespace PortfolioBuilder.Web.Data;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PortfolioDb")
            ?? throw new InvalidOperationException("Connection string 'PortfolioDb' was not found.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}