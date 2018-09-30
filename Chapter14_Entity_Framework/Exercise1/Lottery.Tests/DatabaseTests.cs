using Lottery.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using Lottery.Domain;

namespace Lottery.Tests
{
    public abstract class DatabaseTests : IDisposable
    {
        private SqliteConnection _connection;

        [OneTimeSetUp]
        public void CreateDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using (var context = CreateDbContext())
            {
                context.Database.Migrate();

                //Check if migration succeeded
                try
                {
                    context.Find<LotteryGame>(1);
                    context.Find<Draw>(1);
                }
                catch (SqliteException e)
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine("The migration (creation) of the database is not configured properly.");
                    messageBuilder.AppendLine();
                    messageBuilder.AppendLine(e.Message);
                    Assert.Fail(messageBuilder.ToString());
                }
            }
        }

        [OneTimeTearDown]
        public void DropDatabase()
        {
            using (var context = CreateDbContext())
            {
                context.Database.EnsureDeleted();
            }
            _connection?.Close();
        }

        protected LotteryContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<LotteryContext>()
                .UseSqlite(_connection)
                .Options;

            return new LotteryContext(options);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}