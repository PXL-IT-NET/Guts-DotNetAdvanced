using Bank.Data;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise01",
        @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    internal class CustomerRepositoyTests : DatabaseTestsBase
    {
        private CustomerRepository _repository;
        private Mock<IConnectionFactory> _connectionFactoryMock;
        private SqlConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connectionFactoryMock = new Mock<IConnectionFactory>();
            _connection = Cc();
            _connectionFactoryMock.Setup(factory => factory.CreateSqlConnection()).Returns(_connection);

            _repository = new CustomerRepository(_connectionFactoryMock.Object);
        }

        [MonitoredTest("CustomerRepository - GetAll should return all customers from the database")]
        public void GetAll_ShouldReturnAllCustomersFromDatabase()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();

            //Act
            IList<Customer> retrievedCustomers = null;
            try
            {
                retrievedCustomers = _repository.GetAll();
            }
            catch (SqlNullValueException)
            {
                Assert.Fail("Some of the values your are reading have the value 'null' in the database. " +
                            "Use the 'IsDBNull' method of 'SqlDataReader' to check if the database value of a cell in the row is null. " +
                            "Tip: set a breakpoint the 'GetAll' method and debug this test to find out where the exception happens.");
            }

            //Assert
            Assert.That(retrievedCustomers, Is.Not.Null,
                "The method returns null while there are customers in the database.");
            Assert.That(retrievedCustomers.Count, Is.EqualTo(allOriginalCustomers.Count),
                () => "Not all customers in the database are returned.");

            foreach (var retrievedCustomer in retrievedCustomers)
            {
                var matchingOriginal =
                    allOriginalCustomers.FirstOrDefault(customer =>
                        customer.CustomerId == retrievedCustomer.CustomerId);

                Assert.That(matchingOriginal, Is.Not.Null,
                    () => "The 'CustomerId' property of one or more customers is not correct.");
                Assert.That(retrievedCustomer.Address, Is.EqualTo(matchingOriginal.Address),
                    () => "The 'Address' property of one or more customers is not correct.");
                Assert.That(retrievedCustomer.CellPhone, Is.EqualTo(matchingOriginal.CellPhone),
                    () => "The 'CellPhone' property of one or more customers is not correct.");
                Assert.That(retrievedCustomer.FirstName, Is.EqualTo(matchingOriginal.FirstName),
                    () => "The 'FirstName' property of one or more customers is not correct.");
                Assert.That(retrievedCustomer.Name, Is.EqualTo(matchingOriginal.Name),
                    () => "The 'Name' property of one or more customers is not correct.");
                Assert.That(retrievedCustomer.ZipCode, Is.EqualTo(matchingOriginal.ZipCode),
                    () => "The 'ZipCode' property of one or more customers is not correct.");
            }
        }

        [MonitoredTest("CustomerRepository - GetAll should create and close a database connection")]
        public void GetAll_ShouldCreateAndCloseConnection()
        {
            AssertConnectionIsCreatedAndClosed(() => _repository.GetAll());
        }

        [MonitoredTest("CustomerRepository - Add should add a new customer to the database")]
        public void Add_ShouldAddANewCustomerToTheDatabase()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            var existingCity = GetAllCities().First();
            Customer newCustomer = new CustomerBuilder().WithId(0).WithZipCode(existingCity.ZipCode).Build();

            //Act
            _repository.Add(newCustomer);

            //Assert
            var allCustomersAfterInsert = GetAllCustomers();
            Assert.That(allCustomersAfterInsert.Count, Is.EqualTo(allOriginalCustomers.Count + 1),
                () => "The number of customers in the database should be increased by one.");

            var addedCustomer = allCustomersAfterInsert.FirstOrDefault(customer => customer.Name == newCustomer.Name);
            Assert.That(addedCustomer, Is.Not.Null,
                () => "No customer with the added name can be found in the database afterwards.");
            Assert.That(addedCustomer.CustomerId, Is.GreaterThan(0),
                () => "The customer was added with 'IDENTITY_INSERT' on. " +
                      "You should let the database generate a value for the 'CustomerId' column. " +
                      "Until you fix this problem, other tests might also behave strangely.");
            Assert.That(addedCustomer.Address, Is.EqualTo(newCustomer.Address),
                () => "The 'Address' is not saved correctly.");
            Assert.That(addedCustomer.CellPhone, Is.EqualTo(newCustomer.CellPhone),
                () => "The 'CellPhone' is not saved correctly.");
            Assert.That(addedCustomer.FirstName, Is.EqualTo(newCustomer.FirstName),
                () => "The 'FirstName' is not saved correctly.");
            Assert.That(addedCustomer.Name, Is.EqualTo(newCustomer.Name),
                () => "The 'Name' is not saved correctly.");
            Assert.That(addedCustomer.ZipCode, Is.EqualTo(newCustomer.ZipCode),
                () => "The 'ZipCode' is not saved correctly.");
        }

        [MonitoredTest("CustomerRepository - Add should not be vunerable to SQL injection attacks")]
        public void Add_ShouldNotBeVunerableToSQLInjectionAttacks()
        {
            //Arrange
            var existingCity = GetAllCities().First();
            Customer newCustomer = new CustomerBuilder().WithId(0).WithZipCode(existingCity.ZipCode).Build();
            var sqlInjectionText = "',3500,3500,3500,3500, ''); DELETE FROM dbo.Accounts; --";
            newCustomer.Name = sqlInjectionText;
            newCustomer.FirstName = sqlInjectionText;
            newCustomer.Address = sqlInjectionText;
            newCustomer.CellPhone = sqlInjectionText;

            //Act
            _repository.Add(newCustomer);

            //Assert
            var allAccounts = GetAllAccounts();
            Assert.That(allAccounts.Count, Is.GreaterThan(0),
                () => "A SQL Injection attack that deletes all 'Accounts' from the database succeeded. " +
                      "This may also affect the outcome of other tests.");
        }

        [MonitoredTest("CustomerRepository - Add should create and close a database connection")]
        public void Add_ShouldCreateAndCloseConnection()
        {
            var existingCity = GetAllCities().First();
            Customer newCustomer = new CustomerBuilder().WithId(0).WithZipCode(existingCity.ZipCode).Build();
            AssertConnectionIsCreatedAndClosed(() => _repository.Add(newCustomer));
        }

        [MonitoredTest("CustomerRepository - Add should be able to handle properties that are null")]
        public void Add_ShouldBeAbleToHandlePropertiesThatAreNull()
        {
            //Arrange
            var existingCity = GetAllCities().First();

            Customer newCustomer = new CustomerBuilder()
                .WithId(0)
                .WithZipCode(existingCity.ZipCode)
                .WithNullProperties().Build();

            //Act
            AssertDoesNotThrowSqlParameterException(() => _repository.Add(newCustomer));
        }

        [MonitoredTest("CustomerRepository - Add should set the 'CustomerId' on the inserted customer instance")]
        public void Add_ShouldSetTheCustomerIdOnTheInsertedCustomerInstance()
        {
            //Arrange
            var existingCity = GetAllCities().First();
            Customer newCustomer = new CustomerBuilder().WithId(0).WithZipCode(existingCity.ZipCode).Build();

            //Act
            _repository.Add(newCustomer);

            //Assert
            Assert.That(newCustomer.CustomerId, Is.GreaterThan(0),
                () =>
                    "After calling 'Add', the 'CustomerId' property of the 'newCustomer' object passed as parameter should be greater than zero.");
        }

        [MonitoredTest("CustomerRepository - Add should throw an ArgumentException when the 'ZipCode' is not set")]
        public void Add_ShouldThrowArgumentExceptionWhenZipCodeIsNotSet()
        {
            //Arrange
            Customer newCustomer = new CustomerBuilder().WithId(0).WithZipCode(0).Build();

            //Act + Assert
            Assert.That(() => _repository.Add(newCustomer), Throws.ArgumentException,
                () => "No ArgumentException is thrown when 'ZipCode' is zero.");
        }

        [MonitoredTest("CustomerRepository - Add should throw an ArgumentException when the 'CustomerId' is not zero")]
        public void Add_ShouldThrowArgumentExceptionWhenTheCustomerIdIsNotZero()
        {
            //Arrange
            Customer newCustomer = new CustomerBuilder().WithId().Build();

            //Act + Assert
            Assert.That(() => _repository.Add(newCustomer), Throws.ArgumentException,
                () => "No ArgumentException is thrown when CustomerId is greather than zero");
        }

        [MonitoredTest("CustomerRepository - Update should update an existing customer in the database")]
        public void Update_ShouldUpdateAnExistingCustomerInTheDatabase()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            var existingCity = GetAllCities().First();
            var existingCustomer = allOriginalCustomers.First(c => c.CustomerId > 0);

            var newAddress = Guid.NewGuid().ToString();
            var newCellPhone = Guid.NewGuid().ToString();
            var newFirstName = Guid.NewGuid().ToString();
            var newLastName = Guid.NewGuid().ToString();

            existingCustomer.Address = newAddress;
            existingCustomer.CellPhone = newCellPhone;
            existingCustomer.FirstName = newFirstName;
            existingCustomer.Name = newLastName;
            existingCustomer.ZipCode = existingCity.ZipCode;

            //Act
            _repository.Update(existingCustomer);

            //Assert
            var allCustomersAfterUpdate = GetAllCustomers();

            Assert.That(allCustomersAfterUpdate.Count, Is.EqualTo(allOriginalCustomers.Count),
                () => "After the update the number of customers in the database should be the same as before.");

            var updatedAccount = allCustomersAfterUpdate.First(customer => customer.CustomerId == existingCustomer.CustomerId);
            Assert.That(updatedAccount.Address, Is.EqualTo(newAddress), () => "Address is not updated properly.");
            Assert.That(updatedAccount.CellPhone, Is.EqualTo(newCellPhone), () => "CellPhone is not updated properly.");
            Assert.That(updatedAccount.FirstName, Is.EqualTo(newFirstName), () => "FirstName is not updated properly.");
            Assert.That(updatedAccount.Name, Is.EqualTo(newLastName), () => "Name is not updated properly.");
            Assert.That(updatedAccount.ZipCode, Is.EqualTo(existingCity.ZipCode), () => "ZipCode is not updated properly.");
        }

        [MonitoredTest("CustomerRepository - Update should not be vunerable to SQL injection attacks")]
        public void Update_ShouldNotBeVunerableToSQLInjectionAttacks()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            var existingCustomer = allOriginalCustomers.First(c => c.CustomerId > 0);
            var sqlInjectionText = "'; DELETE FROM dbo.Accounts; --";
            existingCustomer.Name = sqlInjectionText;
            existingCustomer.FirstName = sqlInjectionText;
            existingCustomer.Address = sqlInjectionText;
            existingCustomer.CellPhone = sqlInjectionText;

            //Act
            _repository.Update(existingCustomer);

            //Assert
            var allAccounts = GetAllAccounts();
            Assert.That(allAccounts.Count, Is.GreaterThan(0),
                () => "A SQL Injection attack that deletes all 'Accounts' from the database succeeded. " +
                      "This may also affect the outcome of other tests.");
        }

        [MonitoredTest("CustomerRepository - Update should create and close a database connection")]
        public void Update_ShouldCreateAndCloseConnection()
        {
            var allOriginalCustomers = GetAllCustomers();
            var existingCustomer = allOriginalCustomers.First(c => c.CustomerId > 0);
            AssertConnectionIsCreatedAndClosed(() => _repository.Update(existingCustomer));
        }

        [MonitoredTest("CustomerRepository - Update should be able to handle properties that are null")]
        public void Update_ShouldBeAbleToHandlePropertiesThatAreNull()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            var existingCustomer = allOriginalCustomers.First(c => c.CustomerId > 0);

            existingCustomer.Address = null;
            existingCustomer.CellPhone = null;
            existingCustomer.FirstName = null;
            existingCustomer.Name = null;

            //Act
            AssertDoesNotThrowSqlParameterException(() => _repository.Update(existingCustomer));
        }

        [MonitoredTest("CustomerRepository - Update should throw an ArgumentException when 'ZipCode' is not set")]
        public void Update_ShouldThrowArgumentExceptionWhenZipCodeIsNotSet()
        {
            //Arrange
            Customer existingCustomerWithoutZipCode = new CustomerBuilder().WithId().WithZipCode(0).Build();

            //Act + Assert
            Assert.That(() => _repository.Update(existingCustomerWithoutZipCode), Throws.ArgumentException,
                () => "No ArgumentException is thrown when the 'ZipCode' is zero.");
        }

        [MonitoredTest("CustomerRepository - Update should throw an ArgumentException when 'CustomerId' is not set")]
        public void Update_ShouldThrowArgumentExceptionWhenCustomerIdIsNotSet()
        {
            //Arrange
            Customer newCustomer = new CustomerBuilder().WithId(0).Build();

            //Act + Assert
            Assert.That(() => _repository.Update(newCustomer), Throws.ArgumentException,
                () => "No ArgumentException is thrown when the 'CustomerId' is zero.");
        }

        private void AssertDoesNotThrowSqlParameterException(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Message.Contains("parameter"))
                {
                    Assert.Fail("Make sure that the parameter value for a property that is null is 'DBNull.Value'. " +
                                "Otherwise ADO.NET will think the parameter is not supplied. ");
                }

                Assert.Fail("An unexpected SqlException occurred. " +
                            "Maybe you should first fix some other tests? " +
                            $"Exception message: {sqlException.Message}");
            }
        }

        private void AssertConnectionIsCreatedAndClosed(Action action)
        {
            _connectionFactoryMock.Invocations.Clear();

            action.Invoke();

            _connectionFactoryMock.Verify(factory => factory.CreateSqlConnection(), Times.Once,
                "The 'ConnectionFactory' should be used to create a new 'SqlConnection' each time the repository method is called.");
            Assert.That(_connection.State, Is.EqualTo(ConnectionState.Closed), () => "The created connection is not closed.");
        }
    }
}