using System;
using System.Text;
using Bank.Data;
using Bank.Data.DomainClasses;
using Bank.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

public abstract class DatabaseTests : IDisposable
{
    private SqliteConnection _connection;
    protected Random RandomGenerator;

    protected DatabaseTests()
    {
        RandomGenerator = new Random();
    }

    [OneTimeSetUp]
    public void CreateDatabase()
    {

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using (var context = CreateDbContext())
        {
            //Check if migration succeeds
            try
            {
                context.Database.Migrate();
                context.Find<City>(3500);
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

    protected BankContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BankContext>()
            .UseSqlite(_connection)
            .Options;

        return new BankContext(options);
    }

    protected City CreateExistingCity(BankContext context)
    {
        var existingCity = new City { Name = Guid.NewGuid().ToString(), ZipCode = RandomGenerator.Next(10000, 100000) };
        context.Add(existingCity);
        context.SaveChanges();
        return existingCity;
    }

    protected Customer CreateExistingCustomer(BankContext context)
    {
        City existingCity = CreateExistingCity(context);
        Customer existingCustomer = new CustomerBuilder().WithZipCode(existingCity.ZipCode).Build();
        context.Set<Customer>().Add(existingCustomer);
        context.SaveChanges();
        return existingCustomer;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}