using Bank.Data;
using Bank.Data.DomainClasses;
using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise02",
        @"Bank.Data\DomainClasses\Account.cs;
Bank.Data\DomainClasses\Customer.cs;
Bank.Data\BankContext.cs;
Bank.Data\AccountRepository.cs;
Bank.Data\CityRepository.cs;
Bank.Data\CustomerRepository.cs;
Bank.Business\AccountValidator.cs;
Bank.Business\CustomerValidator.cs;
Bank.UI\AccountsWindow.xaml;
Bank.UI\AccountsWindow.xaml.cs;
Bank.UI\CustomersWindow.xaml;
Bank.UI\CustomersWindow.xaml.cs;
Bank.UI\TransferWindow.xaml;
Bank.UI\TransferWindow.xaml.cs")]
    internal class CustomerRepositoyTests : DatabaseTests
    {
        [MonitoredTest("CustomerRepository - GetAllWithAccounts should return all customers from the database")]
        public void GetAllWithAccounts_ShouldReturnAllCustomersFromDatabase()
        {
            //Arrange
            var random = new Random();
            var expectedCustomers = new List<Customer>();
            var originalAmountOfCustomers = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfCustomers = context.Set<Customer>().Count();

                var existingCity = CreateExistingCity(context);

                for (int i = 0; i < random.Next(5, 21); i++)
                {
                    expectedCustomers.Add(new CustomerBuilder().WithZipCode(existingCity.ZipCode).Build());
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
                Assert.That(allCustomers, Has.Count.EqualTo(originalAmountOfCustomers + expectedCustomers.Count));
                Assert.That(expectedCustomers,
                    Has.All.Matches((Customer expectedCustomer) =>
                        allCustomers.Any(customer => expectedCustomer.Id == customer.Id)));
            }
        }

        [MonitoredTest("CustomerRepository - GetAllWithAccounts should load the accounts of each customer")]
        public void GetAllWithAccounts_ShouldLoadTheAccountsOfEachCustomer()
        {
            //Arrange
            var random = new Random();
            var expectedCustomers = new List<Customer>();
            var originalAmountOfCustomers = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfCustomers = context.Set<Customer>().Count();

                var existingCity = CreateExistingCity(context);

                for (int i = 0; i < random.Next(5, 11); i++)
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
                    "Not all customers are returned.");

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

        [MonitoredTest("CustomerRepository - Add should add a new customer to the database")]
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

            Customer newCustomer = new CustomerBuilder().WithId(0).WithZipCode(existingCity.ZipCode).Build();

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
                Assert.That(addedCustomer.CellPhone, Is.EqualTo(newCustomer.CellPhone),
                    "The 'CellPhone' is not saved correctly.");
                Assert.That(addedCustomer.FirstName, Is.EqualTo(newCustomer.FirstName),
                    "The 'FirstName' is not saved correctly.");
                Assert.That(addedCustomer.ZipCode, Is.EqualTo(newCustomer.ZipCode),
                    "The 'ZipCode' is not saved correctly.");
            }
        }

        [MonitoredTest("CustomerRepository - Add should throw an ArgumentException when the customer already exists in the database")]
        public void Add_ShouldThrowArgumentExceptionWhenTheCustomerAlreadyExists()
        {
            //Arrange
            Customer existingCustomer;
            using (var context = CreateDbContext())
            {
                City existingCity = CreateExistingCity(context);
                existingCustomer = new CustomerBuilder().WithZipCode(existingCity.ZipCode).Build();
                context.Set<Customer>().Add(existingCustomer);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new CustomerRepository(context);

                //Act + Assert
                Assert.That(() => repo.Add(existingCustomer), Throws.ArgumentException);
            }
        }

        [MonitoredTest("CustomerRepository - Update should update an existing customer in the database")]
        public void Update_ShouldUpdateAnExistingCustomerInTheDatabase()
        {
            //Arrange
            IList<Customer> allOriginalCustomers;
            City existingCity;
            Customer existingCustomer;
            using (var context = CreateDbContext())
            {
                existingCity = CreateExistingCity(context);
                existingCustomer = new CustomerBuilder().WithZipCode(existingCity.ZipCode).Build();
                context.Set<Customer>().Add(existingCustomer);
                context.SaveChanges();
                allOriginalCustomers = context.Set<Customer>().ToList();
            }

            var existingCustomerId = existingCustomer.Id;
            var newAddress = Guid.NewGuid().ToString();
            var newCellPhone = Guid.NewGuid().ToString();
            var newFirstName = Guid.NewGuid().ToString();
            var newLastName = Guid.NewGuid().ToString();

            using (var context = CreateDbContext())
            {
                existingCustomer = context.Set<Customer>().Find(existingCustomerId);
                existingCustomer.Address = newAddress;
                existingCustomer.CellPhone = newCellPhone;
                existingCustomer.FirstName = newFirstName;
                existingCustomer.Name = newLastName;
                existingCustomer.ZipCode = existingCity.ZipCode;
                var repo = new CustomerRepository(context);

                //Act
                repo.Update(existingCustomer);
            }

            using (var context = CreateDbContext())
            {
                var updatedCustomer = context.Set<Customer>().Find(existingCustomerId);

                //Assert
                var allCustomers = context.Set<Customer>().ToList();
                Assert.That(allCustomers, Has.Count.EqualTo(allOriginalCustomers.Count),
                    "The amount of customers in the database changed.");

                Assert.That(updatedCustomer.Address, Is.EqualTo(newAddress), "Address is not updated properly.");
                Assert.That(updatedCustomer.CellPhone, Is.EqualTo(newCellPhone), "CellPhone is not updated properly.");
                Assert.That(updatedCustomer.FirstName, Is.EqualTo(newFirstName), "FirstName is not updated properly.");
                Assert.That(updatedCustomer.Name, Is.EqualTo(newLastName), "Name is not updated properly.");
                Assert.That(updatedCustomer.ZipCode, Is.EqualTo(existingCity.ZipCode),  "ZipCode is not updated properly.");
            }
        }

        [MonitoredTest("CustomerRepository - Update should throw an ArgumentException when the customer does not exist in the database")]
        public void Update_ShouldThrowArgumentExceptionWhenTheCustomerDoesNotExists()
        {
            //Arrange
            var newCustomer = new CustomerBuilder().WithId(0).Build();

            using (var context = CreateDbContext())
            {
                var repo = new CustomerRepository(context);

                //Act + Assert
                Assert.That(() => repo.Update(newCustomer), Throws.ArgumentException);
            }
        }
    }
}