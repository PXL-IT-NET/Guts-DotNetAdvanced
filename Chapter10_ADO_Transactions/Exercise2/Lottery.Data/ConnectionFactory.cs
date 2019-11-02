using System.Data.SqlClient;
using Lottery.Data.Interfaces;

namespace Lottery.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        public SqlConnection CreateSqlConnection()
        {
            return null;
        }
    }
}
