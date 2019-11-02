using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Guts.Client.Shared.TestTools;
using Lottery.Domain;
using NUnit.Framework;

namespace Bank.Tests
{
    //Intentionally obfuscated 
    internal abstract class DatabaseTestsBase
    {
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            var script = Solution.Current.GetFileContent(@"Lottery.Tests\DropAndCreateTestDatabase.sql");
            ExecuteScript(script, "MasterConnection");
        }

        [SetUp]
        public void BeforeEachTests()
        {
            var script = Solution.Current.GetFileContent(@"Lottery.Tests\EmptyAndFillTestDatabase.sql");
            ExecuteScript(script);
        }

        protected SqlConnection Cc()
        {
            return Cc("LotteryConnection");
        }

        private SqlConnection Cc(string csn)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[csn].ConnectionString;
            return new SqlConnection(connectionString);
        }

        protected void ExecuteScript(string script, string connectionStringName = "LotteryConnection")
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

        protected IList<LotteryGame> GetAllGames()
        {
            var a = new List<LotteryGame>();
            using (var b = Cc())
            {
                b.Open();
                var c = A(X.A, b);
                using (var d = C(c))
                {
                    int e = B(d, "Id");
                    int f = B(d, "Name");
                    int g = B(d, "NumberOfNumbersInADraw");
                    int h = B(d, "MaximumNumber");

                    while (d.Read())
                    {
                        var i = new LotteryGame
                        {
                            Id = d.GetInt32(e),
                            Name = d.GetString(f),
                            NumberOfNumbersInADraw = d.GetInt32(g),
                            MaximumNumber = d.GetInt32(h),
                        };
                        a.Add(i);
                    }
                }
            }
            return a;
        }

        protected IList<Draw> GetAllDraws()
        {
            var a = new List<Draw>();
            using (var b = Cc())
            {
                b.Open();
                var c = A(X.B, b);
                using (var d = C(c))
                {
                    int e = B(d, "Id");
                    int f = B(d, "LotteryGameId");
                    int g = B(d, "Date");

                    while (d.Read())
                    {
                        var k = new Draw
                        {
                            Id = d.GetInt32(e),
                            LotteryGameId = d.GetInt32(f),
                            Date = d.GetDateTime(g)
                        };
                        a.Add(k);
                    }
                }
            }
            return a;
        }

        protected IList<DrawNumber> GetAllDrawNumbers()
        {
            var a = new List<DrawNumber>();
            using (var b = Cc())
            {
                b.Open();
                var c = A(X.C, b);
                using (var d = C(c))
                {
                    int e = B(d, "DrawId");
                    int f = B(d, "Number");
                    int g = B(d, "Position");

                    while (d.Read())
                    {
                        var j = new DrawNumber
                        {
                            DrawId = d.GetInt32(e),
                            Number = d.GetInt32(f),
                            Position = d.IsDBNull(g) ? null : (int?)d.GetInt32(g)
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

    internal class X
    {
        public const string A = "select * from dbo.LotteryGames";
        public const string B = "select * from dbo.Draws";
        public const string C = "select * from dbo.DrawNumbers";
    }
}