using System.Data.Common;
using DataAccessDapper.Interfaces;
using Microsoft.Extensions.Options;

namespace DataAccessDapper;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IOptionsMonitor<DatabaseSettings> settings;
    public SqlConnectionFactory(IOptionsMonitor<DatabaseSettings> settings)
    {
        this.settings = settings;
    }

    public DbConnection CreateConnection()
    {
        var connection =  GetFactory().CreateConnection();

        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        connection.ConnectionString = settings.CurrentValue.EmployeeDb;

        return connection;
    }

    private static DbProviderFactory GetFactory() => DbProviderFactories.GetFactory("Microsoft.Data.SqlClient");
}

