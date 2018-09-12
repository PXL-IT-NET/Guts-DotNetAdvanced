using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Bank.Data.DomainClasses;
using Bank.Data.DomainClasses.Enums;
using Guts.Client.Shared.TestTools;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using NUnit.Framework;

namespace Bank.Tests
{
    public abstract class DatabaseTestsBase
    {
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            var script = Solution.Current.GetFileContent(@"Bank.Tests\DropAndCreateTestDatabase.sql");

            using (var connection = CreateConnection("MasterConnection"))
            {
                var server = new Server(new ServerConnection(connection)); //makes is possible to execute scripts with GO-statements
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        [SetUp]
        public void BeforeEachTests()
        {
            var script = Solution.Current.GetFileContent(@"Bank.Tests\EmptyAndFillTestDatabase.sql");
            using (var connection = CreateConnection())
            {
                var server = new Server(new ServerConnection(connection)); //makes is possible to execute scripts with GO-statements
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        protected SqlConnection CreateConnection()
        {
            return CreateConnection("BankConnection");
        }

        private SqlConnection CreateConnection(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            return new SqlConnection(connectionString);
        }

        #region Helper functions

        protected IList<City> GetAllCities()
        {
            //Intentionally obfuscated 
            var a = new List<City>();
            using (var b = CreateConnection())
            {
                b.Open();
                var c = new SqlCommand("select * from [dbo].[Cities]", b);
                using (var d = c.ExecuteReader())
                {
                    int e = d.GetOrdinal("ZipCode");
                    int f = d.GetOrdinal("Name");

                    while (d.Read())
                    {
                        var g = new City
                        {
                            ZipCode = d.GetInt32(e),
                            Name = d.IsDBNull(f) ? null : d.GetString(f),
                        };
                        a.Add(g);
                    }
                }
            }
            return a;
        }

        protected IList<Customer> GetAllCustomers()
        {
            //Intentionally obfuscated 
            var a = new List<Customer>();
            using (var b = CreateConnection())
            {
                b.Open();
                var c = new SqlCommand("select * from [dbo].[Customers]", b);
                using (var d = c.ExecuteReader())
                {
                    int e = d.GetOrdinal("CustomerId");
                    int f = d.GetOrdinal("Name");
                    int g = d.GetOrdinal("FirstName");
                    int h = d.GetOrdinal("Address");
                    int i = d.GetOrdinal("CellPhone");
                    int j = d.GetOrdinal("ZipCode");

                    while (d.Read())
                    {
                        var k = new Customer
                        {
                            CustomerId = d.GetInt32(e),
                            Name = d.IsDBNull(f) ? null : d.GetString(f),
                            FirstName = d.IsDBNull(g) ? null : d.GetString(g),
                            Address = d.IsDBNull(h) ? null : d.GetString(h),
                            CellPhone = d.IsDBNull(i) ? null : d.GetString(i),
                            ZipCode = d.GetInt32(j)
                        };
                        a.Add(k);
                    }
                }
            }
            return a;
        }

        protected IList<Account> GetAllAccounts()
        {
            //Intentionally obfuscated 
            var a = new List<Account>();
            using (var b = CreateConnection())
            {
                b.Open();
                var c = new SqlCommand("select * from [dbo].[Accounts]", b);
                using (var d = c.ExecuteReader())
                {
                    int e = d.GetOrdinal("Id");
                    int f = d.GetOrdinal("AccountNumber");
                    int g = d.GetOrdinal("Balance");
                    int h = d.GetOrdinal("AccountType");
                    int i = d.GetOrdinal("CustomerId");
                    
                    while (d.Read())
                    {
                        var j = new Account
                        {
                            Id = d.GetInt32(e),
                            AccountNumber = d.IsDBNull(f) ? null : d.GetString(f),
                            Balance = d.GetDecimal(g),
                            AccountType = d.GetFieldValue<AccountType>(h),
                            CustomerId = d.GetInt32(i)
                        };
                        a.Add(j);
                    }
                }
            }
            return a;
        }

        #endregion
    }
}