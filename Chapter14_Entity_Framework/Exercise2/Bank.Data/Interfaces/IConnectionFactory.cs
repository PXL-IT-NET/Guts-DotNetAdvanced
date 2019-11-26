using System.Data.SqlClient;

namespace Bank.Data.Interfaces
{
    public interface IConnectionFactory
    {
        SqlConnection CreateSqlConnection();
    }
}