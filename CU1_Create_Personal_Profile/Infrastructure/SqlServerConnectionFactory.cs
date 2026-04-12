using Microsoft.Data.SqlClient;

namespace CestaJusta.CU1.CreateProfile.Infrastructure;

public sealed class SqlServerConnectionFactory
{
    private readonly string connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public SqlConnection CreateConnection() => new(connectionString);
}