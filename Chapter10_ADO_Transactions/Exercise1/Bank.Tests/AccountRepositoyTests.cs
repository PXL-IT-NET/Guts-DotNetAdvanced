using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Bank.Data;
using Bank.Data.DomainClasses;
using Bank.Data.DomainClasses.Enums;
using Bank.Data.Interfaces;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Moq;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise01",
        @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    internal class AccountRepositoyTests : DatabaseTestsBase
    {
        private AccountRepository _repository;
        private Random _random;
        private Mock<IConnectionFactory> _connectionFactoryMock;
        private SqlConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connectionFactoryMock = new Mock<IConnectionFactory>();
            _connection = Cc();
            _connectionFactoryMock.Setup(factory => factory.CreateSqlConnection()).Returns(_connection);

            _repository = new AccountRepository(_connectionFactoryMock.Object);

            _random = new Random();
        }

        [MonitoredTest("AccountRepository - GetAllAccountsOfCustomer should return all customer accounts from database")]
        public void GetAllAccountsOfCustomer_ShouldReturnAllCustomerAccountsFromDatabase()
        {
            //Arrange
            var customerId = GetAllCustomers().First(c => c.CustomerId > 0).CustomerId;
            var allAccounts = GetAllAccounts();
            var allAccountsOfCustomer = allAccounts.Where(account => account.CustomerId == customerId).ToList();

            //Act
            IList<Account> retrievedAccounts = null;
            try
            {
                retrievedAccounts = _repository.GetAllAccountsOfCustomer(customerId);
            }
            catch (SqlNullValueException)
            {
                Assert.Fail("Some of the values your are reading have the value 'null' in the database. " +
                            "Use the 'IsDBNull' method of 'SqlDataReader' to check if the database value of a cell in the row is null. " +
                            "Tip: set a breakpoint the 'GetAllAccountsOfCustomer' method and debug this test to find out where the exception happens.");
            }

            //Assert
            Assert.That(retrievedAccounts, Is.Not.Null, () => "No accounts are returned.");
            Assert.That(retrievedAccounts.Count, Is.EqualTo(allAccountsOfCustomer.Count),
                () =>
                    "Only the accounts linked to the customer should be returned. " +
                    "You return too few or too many accounts.");
            foreach (var retrievedAccount in retrievedAccounts)
            {
                var matchingOriginal =
                    allAccountsOfCustomer.FirstOrDefault(account => account.Id == retrievedAccount.Id);

                Assert.That(matchingOriginal, Is.Not.Null,
                    () => "The 'Id' property of one or more accounts is not correct.");
                Assert.That(retrievedAccount.CustomerId, Is.EqualTo(customerId),
                    () => "The 'CustomerId' property of one or more accounts is not correct.");
                Assert.That(retrievedAccount.AccountNumber, Is.EqualTo(matchingOriginal.AccountNumber),
                    () => "The 'AccountNumber' property of one or more accounts is not correct.");
                Assert.That(retrievedAccount.AccountType, Is.EqualTo(matchingOriginal.AccountType),
                    () => "The 'AccountType' property of one or more accounts is not correct.");
                Assert.That(retrievedAccount.Balance, Is.EqualTo(matchingOriginal.Balance),
                    () => "The 'Balance' property of one or more accounts is not correct.");
            }
        }

        [MonitoredTest("AccountRepository - GetAllAccountsOfCustomer should create and close a database connection")]
        public void GetAllAccountsOfCustomer_ShouldCreateAndCloseConnection()
        {
            var customerId = GetAllCustomers().First(c => c.CustomerId > 0).CustomerId;
            AssertConnectionIsCreatedAndClosed(() => _repository.GetAllAccountsOfCustomer(customerId));
        }

        [MonitoredTest("AccountRepository - Add should add a new account to the database for the correct customer")]
        public void Add_ShouldAddANewAccountToTheDatabaseForTheCorrectCustomer()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            int existingCustomerId = allOriginalCustomers.First(c => c.CustomerId > 0).CustomerId;
            Account newAccount = new AccountBuilder().WithCustomerId(existingCustomerId).Build();

            var allOriginalAccountsOfCustomer =
                GetAllAccounts().Where(account => account.CustomerId == existingCustomerId).ToList();

            //Act
            _repository.Add(newAccount);

            //Assert
            var allAccounts = GetAllAccounts();
            var allAccountsOfCustomer = allAccounts.Where(account => account.CustomerId == existingCustomerId).ToList();

            Assert.That(allAccountsOfCustomer.Count, Is.EqualTo(allOriginalAccountsOfCustomer.Count + 1),
                () => "The number of accounts in the database should be increased by one.");

            var addedAccount = allAccounts.FirstOrDefault(account => account.AccountNumber == newAccount.AccountNumber);
            Assert.That(addedAccount, Is.Not.Null,
                () => "No account with the added account number can be found in the database afterwards.");
            Assert.That(addedAccount.Id, Is.GreaterThan(0),
                () => "The account was added with 'IDENTITY_INSERT' on. " +
                      "You should let the database generate a value for the 'Id' column. " +
                      "Until you fix this problem, other tests might also behave strangely.");
            Assert.That(addedAccount.CustomerId, Is.EqualTo(existingCustomerId),
                () => "The customerId of the added account is not correct.");
            Assert.That(addedAccount.AccountType, Is.EqualTo(newAccount.AccountType),
                () => "The 'AccountType' is not saved correctly.");
            Assert.That(addedAccount.Balance, Is.EqualTo(newAccount.Balance),
                () => "The 'Balance' is not saved correctly.");
        }

        [MonitoredTest("AccountRepository - Add should not be vunerable to SQL injection attacks")]
        public void Add_ShouldNotBeVunerableToSQLInjectionAttacks()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            int existingCustomerId = allOriginalCustomers.First(c => c.CustomerId > 0).CustomerId;
            Account newAccount = new AccountBuilder().WithCustomerId(existingCustomerId).Build();
            var sqlInjectionText = $"',{existingCustomerId},{(int)AccountType.PaymentAccount},{existingCustomerId}); DELETE FROM dbo.Accounts; --";
            newAccount.AccountNumber = sqlInjectionText;

            //Act
            _repository.Add(newAccount);

            //Assert
            var allAccounts = GetAllAccounts();
            Assert.That(allAccounts.Count, Is.GreaterThan(0),
                () => "A SQL Injection attack that deletes all 'Accounts' from the database succeeded. " +
                      "This may also affect the outcome of other tests.");
        }

        [MonitoredTest("AccountRepository - Add should create and close a database connection")]
        public void Add_ShouldCreateAndCloseConnection()
        {
            var allOriginalCustomers = GetAllCustomers();
            int existingCustomerId = allOriginalCustomers.First(c => c.CustomerId > 0).CustomerId;
            Account newAccount = new AccountBuilder().WithCustomerId(existingCustomerId).Build();
            AssertConnectionIsCreatedAndClosed(() => _repository.Add(newAccount));
        }

        [MonitoredTest("AccountRepository - Add should be able to handle null for account number")]
        public void Add_ShouldBeAbleToHandleNullForAccountNumber()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            int existingCustomerId = allOriginalCustomers.First().CustomerId;

            Account newAccount = new AccountBuilder()
                .WithCustomerId(existingCustomerId)
                .WithAccountNumber(null)
                .Build();

            //Act + Assert
            AssertDoesNotThrowSqlParameterException(() => _repository.Add(newAccount));
        }

        [MonitoredTest("AccountRepository - Add should set the id on the inserted account instance")]
        public void Add_ShouldSetTheIdOnTheInsertedAccountInstance()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            int existingCustomerId = allOriginalCustomers.First().CustomerId;

            Account newAccount = new AccountBuilder().WithId(0).WithCustomerId(existingCustomerId).Build();

            //Act
            _repository.Add(newAccount);

            //Assert
            Assert.That(newAccount.Id, Is.GreaterThan(0),
                () =>
                    "After calling 'Add', the 'Id' property of the 'newAccount' object passed as parameter should be greater than zero.");
        }

        [MonitoredTest("AccountRepository - Add should throw an ArgumentException when CustomerId of the account is not set")]
        public void Add_ShouldThrowArgumentExceptionWhenCustomerIdIsNotSet()
        {
            //Arrange
            Account newAccount = new AccountBuilder().WithCustomerId(0).Build();

            //Act + Assert
            Assert.That(() => _repository.Add(newAccount), Throws.ArgumentException,
                () => "No ArgumentException is thrown when 'CustomerId' is zero.");
        }

        [MonitoredTest("AccountRepository - Add should throw an ArgumentException when the Id of the account is greather than zero")]
        public void Add_ShouldThrowArgumentExceptionWhenTheAccountIdIsNotZero()
        {
            //Arrange
            Account newAccount = new AccountBuilder().WithCustomerId().WithId().Build();

            //Act + Assert
            Assert.That(() => _repository.Add(newAccount), Throws.ArgumentException,
                () => "No ArgumentException is thrown when Id is greather than zero");
        }

        [MonitoredTest("AccountRepository - Update should update an existing account in the database")]
        public void Update_ShouldUpdateAnExistingAccountInTheDatabase()
        {
            //Arrange
            var allOriginalAccounts = GetAllAccounts();
            var existingAccount = allOriginalAccounts.First(a => a.Id > 0);

            var newAccountNumber = Guid.NewGuid().ToString();
            var newBalance = _random.Next(0, int.MaxValue);
            var newAccountType = AccountType.PremiumAccount;

            existingAccount.AccountNumber = newAccountNumber;
            existingAccount.Balance = newBalance;
            existingAccount.AccountType = newAccountType;

            //Act
            _repository.Update(existingAccount);

            //Assert
            var allAccountsAfterUpdate = GetAllAccounts();

            Assert.That(allAccountsAfterUpdate.Count, Is.EqualTo(allOriginalAccounts.Count),
                () => "After the update the number of accounts in the database should be the same as before.");

            var updatedAccount = allAccountsAfterUpdate.First(account => account.Id == existingAccount.Id);
            Assert.That(updatedAccount.AccountNumber, Is.EqualTo(newAccountNumber), () => "AccountNumber is not updated properly.");
            Assert.That(updatedAccount.Balance, Is.EqualTo(newBalance), () => "Balance is not updated properly.");
            Assert.That(updatedAccount.AccountType, Is.EqualTo(newAccountType), () => "AccountType is not updated properly.");
        }

        [MonitoredTest("AccountRepository - Update should not be vunerable to SQL injection attacks")]
        public void Update_ShouldNotBeVunerableToSQLInjectionAttacks()
        {
            //Arrange
            var allOriginalAccounts = GetAllAccounts();
            var existingAccount = allOriginalAccounts.First(a => a.Id > 0);
            var sqlInjectionText = "'; DELETE FROM dbo.Accounts; --";
            existingAccount.AccountNumber = sqlInjectionText;

            //Act
            _repository.Update(existingAccount);

            //Assert
            var allAccounts = GetAllAccounts();
            Assert.That(allAccounts.Count, Is.GreaterThan(0),
                () => "A SQL Injection attack that deletes all 'Accounts' from the database succeeded. " +
                      "This may also affect the outcome of other tests.");
        }

        [MonitoredTest("AccountRepository - Update should create and close a database connection")]
        public void Update_ShouldCreateAndCloseConnection()
        {
            var allOriginalAccounts = GetAllAccounts();
            var existingAccount = allOriginalAccounts.First(a => a.Id > 0);
            AssertConnectionIsCreatedAndClosed(() => _repository.Update(existingAccount));
        }

        [MonitoredTest("AccountRepository - Update should be able to handle null for account number")]
        public void Update_ShouldBeAbleToHandleNullForAccountNumber()
        {
            //Arrange
            var existingAccount = GetAllAccounts().First(a => a.Id > 0);
            existingAccount.AccountNumber = null;

            //Act + Assert
            AssertDoesNotThrowSqlParameterException(() => _repository.Update(existingAccount));
        }

        [MonitoredTest("AccountRepository - Update should throw an ArgumentException when the CustomerId is not set")]
        public void Update_ShouldThrowArgumentExceptionWhenCustomerIdIsNotSet()
        {
            //Arrange
            Account existingAccountWithoutCustomer = new AccountBuilder().WithId().WithCustomerId(0).Build();

            //Act + Assert
            Assert.That(() => _repository.Update(existingAccountWithoutCustomer), Throws.ArgumentException,
                () => "No ArgumentException is thrown when the CustomerId is zero.");
        }

        [MonitoredTest("AccountRepository - Update should throw an ArgumentException when the Id of the account is zero")]
        public void Update_ShouldThrowArgumentExceptionWhenTheAccountIdIsZero()
        {
            //Arrange
            Account newAccount = new AccountBuilder().WithCustomerId().WithId(0).Build();

            //Act + Assert
            Assert.That(() => _repository.Update(newAccount), Throws.ArgumentException,
                () => "No ArgumentException is thrown when the Id is zero.");
        }

        [MonitoredTest("AccountRepository - TransferMoney should correctly transfer a valid amount")]
        public void TransferMoney_ShouldCorrectlyTransferAValidAmount()
        {
            //Arrange
            var allOriginalAccounts = GetAllAccounts();
            var originalFromAccount = allOriginalAccounts.First(a => a.Id > 0);
            var originalToAccount = allOriginalAccounts.First(a => a.Id > 0 && a.Id != originalFromAccount.Id);
            var validAmount = _random.Next(1, Convert.ToInt32(originalFromAccount.Balance));

            //Act
            _repository.TransferMoney(originalFromAccount.Id, originalToAccount.Id, validAmount);

            //Assert
            var allAccounts = GetAllAccounts();
            var updatedFromAccount = allAccounts.First(a => a.Id > 0);
            var updatedToAccount = allAccounts.First(a => a.Id > 0 && a.Id != originalFromAccount.Id);

            Assert.That(updatedFromAccount.Balance, Is.EqualTo(originalFromAccount.Balance - validAmount),
                () => "The balance of the 'From' account is not correct.");
            Assert.That(updatedToAccount.Balance, Is.EqualTo(originalToAccount.Balance + validAmount),
                () => "The balance of the 'To' account is not correct.");
        }

        [MonitoredTest("AccountRepository - TransferMoney should create and close a database connection")]
        public void TransferMoney_ShouldCreateAndCloseConnection()
        {
            var allOriginalAccounts = GetAllAccounts();
            var originalFromAccount = allOriginalAccounts.First(a => a.Id > 0);
            var originalToAccount = allOriginalAccounts.First(a => a.Id > 0 && a.Id != originalFromAccount.Id);
            var validAmount = _random.Next(1, Convert.ToInt32(originalFromAccount.Balance));
            AssertConnectionIsCreatedAndClosed(() => _repository.TransferMoney(originalFromAccount.Id, originalToAccount.Id, validAmount));
        }

        [MonitoredTest("AccountRepository - TransferMoney should use a transaction")]
        public void TransferMoney_ShouldUseATransaction()
        {
            var sourceCode = Solution.Current.GetFileContent(@"Bank.Data\AccountRepository.cs");
            sourceCode = CodeCleaner.StripComments(sourceCode);

            Assert.That(sourceCode, Contains.Substring(".BeginTransaction();"),
                () => "No method call found in the source code that begins a transaction.");
            Assert.That(sourceCode, Contains.Substring(".Transaction ="),
                () => "No code found in the source code that links the transaction to the commands.");
            Assert.That(sourceCode, Contains.Substring(".Commit();"),
                () => "No code found in the source code that commits the transaction.");
            Assert.That(sourceCode, Contains.Substring(".Rollback();"),
                () => "No code found in the source code that rolls back the transaction.");
        }

        [MonitoredTest("AccountRepository - TransferMoney should rollback the transaction when the update of the 'To' account fails")]
        public void TransferMoney_ShouldRollbackTransactionWhenTheUpdateOfTheToAccountFails()
        {
            //Arrange
            var allOriginalAccounts = GetAllAccounts();
            var originalFromAccount = allOriginalAccounts.First(a => a.Id > 0);
            var invalidToAccountId = -1;
            var amount = _random.Next(1, Convert.ToInt32(originalFromAccount.Balance));

            //Act
            _repository.TransferMoney(originalFromAccount.Id, invalidToAccountId, amount);

            //Assert
            var allAccounts = GetAllAccounts();
            var updatedFromAccount = allAccounts.First();

            Assert.That(updatedFromAccount.Balance, Is.EqualTo(originalFromAccount.Balance),
                () => "The update of the balance the 'From' account is persisted in the database. " +
                      "This should not be allowed when the 'Id' of the 'To' account is invalid (e.g. -1) and an exception happens. " +
                      "In that case the transaction should be rolled back.");
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
