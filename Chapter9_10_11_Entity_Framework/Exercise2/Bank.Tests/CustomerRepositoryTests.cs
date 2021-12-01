using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Domain;
using Bank.Infrastructure;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.Infrastructure\CustomerRepository.cs")]
    internal class CustomerRepositoryTests : DatabaseTests
    {
        private static Random _random = new Random();

        [MonitoredTest("CustomerRepository - GetAllWithAccounts - Should return all customers from the database")]
        public void GetAllWithAccounts_ShouldReturnAllCustomersFromDatabase()
        {
            //Arrange
            var expectedCustomers = new List<Customer>();
            var originalAmountOfCustomers = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfCustomers = context.Set<Customer>().Count();

                for (int i = 0; i < _random.Next(5, 21); i++)
                {
                    expectedCustomers.Add(CreateExistingCustomer(context));
                }
            }

            using (var context = CreateDbContext())
            {
                var repo = new CustomerRepository(context);

                //Act
                var allCustomers = repo.GetAllWithAccounts();

                //Assert
                Assert.That(allCustomers, Has.Count.EqualTo(originalAmountOfCustomers + expectedCustomers.Count),
                    "An incorrect amount of customers is returned.");
                Assert.That(expectedCustomers,
                    Has.All.Matches((Customer expectedCustomer) =>
                        allCustomers.Any(customer => expectedCustomer.Id == customer.Id)),
                    "Not all customers are returned.");
            }
        }

        [MonitoredTest("CustomerRepository - GetAllWithAccounts - Should load the accounts of each customer")]
        public void GetAllWithAccounts_ShouldLoadTheAccountsOfEachCustomer()
        {
            //Arrange
            var expectedCustomers = new List<Customer>();
            var originalAmountOfCustomers = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfCustomers = context.Set<Customer>().Count();
                City existingCity = CreateExistingCity(context);
                for (int i = 0; i < _random.Next(5, 11); i++)
                {
                    expectedCustomers.Add(
                        new CustomerBuilder().WithZipCode(existingCity.ZipCode).WithAccounts().Build());
                }

                context.AddRange(expectedCustomers);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new CustomerRepository(context);

                //Act
                var allCustomers = repo.GetAllWithAccounts();

                //Assert
                Assert.That(allCustomers, Has.Count.EqualTo(originalAmountOfCustomers + expectedCustomers.Count),
                    "An incorrect amount of customers is returned.");

                var allCustomersWithAccounts = allCustomers.Where(c => c.TryGetAccounts()?.Any() ?? false).ToList();
                Assert.That(allCustomersWithAccounts, Has.Count.EqualTo(expectedCustomers.Count),
                    "Not all customers have their accounts loaded.");

                var expectedAccountCount = expectedCustomers.Sum(c =>
                {
                    var accounts = c.TryGetAccounts() ?? Enumerable.Empty<Account>();
                    return accounts.Count();
                });
                var accountCount = allCustomersWithAccounts.Sum(c =>
                {
                    var accounts = c.TryGetAccounts() ?? Enumerable.Empty<Account>();
                    return accounts.Count();
                });
                Assert.That(accountCount, Is.EqualTo(expectedAccountCount), "Not all accounts of all customers are loaded.");
            }
        }

        [MonitoredTest("CustomerRepository - Add - Should add a new customer to the database")]
        public void Add_ShouldAddANewCustomerToTheDatabase()
        {
            //Arrange
            IList<Customer> allOriginalCustomers;
            City existingCity;
            using (var context = CreateDbContext())
            {
                allOriginalCustomers = context.Set<Customer>().ToList();
                existingCity = CreateExistingCity(context);
            }

            Customer newCustomer = new CustomerBuilder().WithZipCode(existingCity.ZipCode).WithId(0).Build();

            using (var context = CreateDbContext())
            {
                var repo = new CustomerRepository(context);

                //Act
                repo.Add(newCustomer);

                //Assert
                var allCustomers = context.Set<Customer>().ToList();
                Assert.That(allCustomers, Has.Count.EqualTo(allOriginalCustomers.Count + 1),
                    "No customer is added in the database.");
                var addedCustomer = allCustomers.FirstOrDefault(c => c.Name == newCustomer.Name);
                Assert.That(addedCustomer, Is.Not.Null,
                    "No customer with the added name can be found in the database afterwards.");
                Assert.That(addedCustomer.Id, Is.GreaterThan(0),
                    "The Id of the added customer must be greater than zero.");
                Assert.That(addedCustomer.Address, Is.EqualTo(newCustomer.Address),
                    "The 'Address' is not saved correctly.");
                Assert.That(addedCustomer.FirstName, Is.EqualTo(newCustomer.FirstName),
                    "The 'FirstName' is not saved correctly.");
            }
        }

        [MonitoredTest("CustomerRepository - Add - Should throw an ArgumentException when the customer already exists in the database")]
        public void Add_ShouldThrowArgumentExceptionWhenTheCustomerAlreadyExists()
        {
            //Arrange
            Customer existingCustomer;
            using (var context = CreateDbContext())
            {
                existingCustomer = CreateExistingCustomer(context);
            }

            using (var context = CreateDbContext())
            {
                var repo = new CustomerRepository(context);

                //Act + Assert
                Assert.That(() => repo.Add(existingCustomer), Throws.ArgumentException);
            }
        }
    }
}