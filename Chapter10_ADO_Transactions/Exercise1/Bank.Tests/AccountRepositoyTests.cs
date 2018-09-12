using System;
using System.Collections.Generic;
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
    [MonitoredTestFixture("dotnet2", 10, 1, @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    public class AccountRepositoyTests : DatabaseTestsBase
    {
        private AccountRepository _repository;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            connectionFactoryMock.Setup(factory => factory.CreateSqlConnection()).Returns(CreateConnection);

            _repository = new AccountRepository(connectionFactoryMock.Object);
   
            _random = new Random();
        }

        [MonitoredTest("AccountRepository - GetAllAccountsOfCustomer should return all customer accounts from database")]
        public void GetAllAccountsOfCustomer_ShouldReturnAllCustomerAccountsFromDatabase()
        {
            //Arrange
            var customerId = GetAllCustomers().First().CustomerId;
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

        [MonitoredTest("AccountRepository - Add should add a new account to the database for the correct customer")]
        public void Add_ShouldAddANewAccountToTheDatabaseForTheCorrectCustomer()
        {
            //Arrange
            var allOriginalCustomers = GetAllCustomers();
            int existingCustomerId = allOriginalCustomers.First().CustomerId;

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
            Assert.That(addedAccount.CustomerId, Is.EqualTo(existingCustomerId),
                () => "The customerId of the added account is not correct.");
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
            var existingAccount = allOriginalAccounts.First();

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
            var originalFromAccount = allOriginalAccounts.First();
            var originalToAccount = allOriginalAccounts.ElementAt(1);
            var validAmount = _random.Next(1, Convert.ToInt32(originalFromAccount.Balance));
            
            //Act
            _repository.TransferMoney(originalFromAccount.Id, originalToAccount.Id, validAmount);

            //Assert
            var allAccounts = GetAllAccounts();
            var updatedFromAccount = allAccounts.First();
            var updatedToAccount = allAccounts.ElementAt(1);

            Assert.That(updatedFromAccount.Balance, Is.EqualTo(originalFromAccount.Balance - validAmount),
                () => "The balance of the 'From' account is not correct.");
            Assert.That(updatedToAccount.Balance, Is.EqualTo(originalToAccount.Balance + validAmount),
                () => "The balance of the 'To' account is not correct.");
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
            var originalFromAccount = allOriginalAccounts.First();
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
    }
}
