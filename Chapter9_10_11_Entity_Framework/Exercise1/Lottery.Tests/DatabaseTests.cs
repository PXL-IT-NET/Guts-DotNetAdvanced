using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Text;
using Lottery.Domain;
using Lottery.Infrastructure;

namespace Lottery.Tests
{
    internal abstract class DatabaseTests : IDisposable
    {
        private SqliteConnection _connection;
        private string _migrationError;

        public DatabaseTests()
        {
            _migrationError = string.Empty;
        }

        [OneTimeSetUp]
        public void CreateDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using (var context = CreateDbContext(false))
            {
                context.Database.Migrate();

                //Check if migration succeeded
                try
                {
                    context.Find<LotteryGame>(1);
                    context.Find<Draw>(1);
                }
                catch (Exception e)
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine("The migration (creation) of the database is not configured properly.");
                    messageBuilder.AppendLine();
                    messageBuilder.AppendLine(e.Message);
                    _migrationError = messageBuilder.ToString();
                }
            }
        }

        [OneTimeTearDown]
        public void DropDatabase()
        {
            using (var context = CreateDbContext(false))
            {
                context.Database.EnsureDeleted();
            }
            _connection?.Close();
        }

        protected LotteryContext CreateDbContext(bool assertMigration = true)
        {
            if (assertMigration)
            {
                AssertMigratedSuccessfully();
            }

            var options = new DbContextOptionsBuilder<LotteryContext>()
                .UseSqlite(_connection)
                .Options;

            return new LotteryContext(options);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private void AssertMigratedSuccessfully()
        {
            if (!string.IsNullOrEmpty(_migrationError))
            {
                Assert.Fail(_migrationError);
            }
        }
    }
}