using System.Data.Common;

namespace DataAccessDapper.Interfaces;

public interface ISqlConnectionFactory
{
    public DbConnection CreateConnection();
}