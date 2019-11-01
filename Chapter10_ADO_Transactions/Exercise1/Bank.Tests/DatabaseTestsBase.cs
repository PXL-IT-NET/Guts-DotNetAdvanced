using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Bank.Data.DomainClasses;
using Bank.Data.DomainClasses.Enums;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Bank.Tests
{
    //Intentionally obfuscated 
    internal abstract class DatabaseTestsBase
    {
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            var script = Solution.Current.GetFileContent(@"Bank.Tests\DropAndCreateTestDatabase.sql");
            ExecuteScript(script, "MasterConnection");
        }

        [SetUp]
        public void BeforeEachTests()
        {
            var script = Solution.Current.GetFileContent(@"Bank.Tests\EmptyAndFillTestDatabase.sql");
            ExecuteScript(script, "BankConnection");
        }

        protected SqlConnection Cc()
        {
            return Cc("BankConnection");
        }

        private SqlConnection Cc(string csn)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[csn].ConnectionString;
            return new SqlConnection(connectionString);
        }

        private void ExecuteScript(string script, string connectionStringName)
        {
            var c = script.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            using (var a = Cc(connectionStringName))
            {
                a.Open();
                foreach (var d in c)
                {
                    var b = new SqlCommand(d, a);
                    b.ExecuteNonQuery();
                }
                a.Close();
            }
        }

        #region Helper functions

        protected IList<City> GetAllCities()
        {
            var a = new List<City>();
            using (var b = Cc())
            {
                b.Open();
                var c = A(X.A, b);
                using (var d = C(c))
                {
                    int e = B(d, "ZipCode");
                    int f = B(d, "Name");

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
            var a = new List<Customer>();
            using (var b = Cc())
            {
                b.Open();
                var c = A(X.B, b);
                using (var d = C(c))
                {
                    int e = B(d,"CustomerId");
                    int f = B(d,"Name");
                    int g = B(d,"FirstName");
                    int h = B(d,"Address");
                    int i = B(d,"CellPhone");
                    int j = B(d, "ZipCode");

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
            var a = new List<Account>();
            using (var b = Cc())
            {
                b.Open();
                var c = A(X.C, b);
                using (var d = C(c))
                {
                    int e = B(d,"Id");
                    int f = B(d,"AccountNumber");
                    int g = B(d,"Balance");
                    int h = B(d,"AccountType");
                    int i = B(d, "CustomerId");

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

        private SqlCommand A(string a, SqlConnection b)
        {
            return new SqlCommand(a, b);
        }

        private int B(SqlDataReader a, string b)
        {
            return a.GetOrdinal(b);
        }

        private SqlDataReader C(SqlCommand a)
        {
            return a.ExecuteReader();
        }
        #endregion
    }
}