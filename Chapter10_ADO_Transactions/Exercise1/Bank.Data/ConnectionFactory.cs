using System;
using System.Configuration;
using System.Data.SqlClient;
using Bank.Data.Interfaces;

namespace Bank.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        public SqlConnection CreateSqlConnection()
        {
            throw new NotImplementedException();
        }
    }
}