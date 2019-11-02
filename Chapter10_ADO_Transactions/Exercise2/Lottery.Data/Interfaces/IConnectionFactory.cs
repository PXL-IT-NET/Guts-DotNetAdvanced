using System.Data.SqlClient;

namespace Lottery.Data.Interfaces
{
    public interface IConnectionFactory
    {
        SqlConnection CreateSqlConnection();
    }
}