using System;
using System.Linq;
using System.Text;
using Bank.Domain;
using Bank.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Bank.Tests
{
    public abstract class DatabaseTests : IDisposable
    {
        private SqliteConnection _connection;
        private string _migrationError;

        protected Random RandomGenerator;

        protected DatabaseTests()
        {
            RandomGenerator = new Random();
            _migrationError = string.Empty;
        }

        [OneTimeSetUp]
        public void CreateDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using (var context = CreateDbContext(false))
            {
                //Check if migration succeeds
                try
                {
                    context.Database.Migrate();
                    City firstCity = context.Set<City>().FirstOrDefault();
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

        public void Dispose()
        {
            _connection?.Dispose();
        }
        
        internal BankContext CreateDbContext(bool assertMigration = true)
        {
            if (assertMigration)
            {
                AssertMigratedSuccessfully();
            }

            var options = new DbContextOptionsBuilder<BankContext>()
                .UseSqlite(_connection)
                .Options;

            return new BankContext(options);
        }

        internal City CreateExistingCity(BankContext context)
        {
            var existingCity = new City { Name = Guid.NewGuid().ToString(), ZipCode = RandomGenerator.Next(10000, 100000) };
            context.Add(existingCity);
            context.SaveChanges();
            return existingCity;
        }

        internal Customer CreateExistingCustomer(BankContext context)
        {
            City existingCity = CreateExistingCity(context);
            Customer existingCustomer = new CustomerBuilder().WithZipCode(existingCity.ZipCode).Build();
            context.Set<Customer>().Add(existingCustomer);
            context.SaveChanges();
            return existingCustomer;
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