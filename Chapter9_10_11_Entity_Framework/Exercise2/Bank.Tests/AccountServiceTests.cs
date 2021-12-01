using System;
using System.Collections.Generic;
using Bank.AppLogic;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.AppLogic\AccountService.cs")]
    public class AccountServiceTests
    {
        private static readonly Random Random = new Random();
        private AccountService _service;
        private Mock<IAccountRepository> _accountRepositoryMock;

        [SetUp]
        public void BeforeEachTest()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _service = new AccountService(_accountRepositoryMock.Object);
        }

        [MonitoredTest("AccountService - AddNewAccountForCustomer - Customer already has an account of that type - Should fail")]
        public void AddNewAccountForCustomer_CustomerAlreadyHasAnAccountOfThatType_ShouldFail()
        {
            //Arrange
            Customer customer = new CustomerBuilder().WithId().Build();
            AccountType type = Random.NextAccountType();
            string accountNumber = Guid.NewGuid().ToString();
            var accounts = new List<Account>()
            {
                new AccountBuilder().WithCustomerId(customer.Id).WithType(type).Build()
            };
            customer.TrySetAccounts(accounts);

            //Act
            Result result = _service.AddNewAccountForCustomer(customer, accountNumber, type);

            //Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Contains.Substring("type").IgnoreCase, "The result message should contain the word 'type'.");
            _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
        }

        [MonitoredTest("AccountService - AddNewAccountForCustomer - Account number already in use - Should fail")]
        public void AddNewAccountForCustomer_AccountNumberAlreadyInUse_ShouldFail()
        {
            //Arrange
            Customer customer = new CustomerBuilder().WithId().Build();
            AccountType type = Random.NextAccountType();
            string accountNumber = Guid.NewGuid().ToString();

            var existingAccount = new AccountBuilder().Build();
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(accountNumber)).Returns(existingAccount);

            //Act
            Result result = _service.AddNewAccountForCustomer(customer, accountNumber, type);

            //Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Contains.Substring("exist").IgnoreCase, "The result message should contain the word 'exist'.");
            _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
        }

        [MonitoredTest("AccountService - AddNewAccountForCustomer - Should use repository to add new account")]
        public void AddNewAccountForCustomer_ShouldUseRepositoryToAddNewAccount()
        {
            //Arrange
            Customer customer = new CustomerBuilder().WithId().Build();
            AccountType type = Random.NextAccountType();
            string accountNumber = Guid.NewGuid().ToString();

            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(It.IsAny<string>())).Returns(() => null);

            //Act
            Result result = _service.AddNewAccountForCustomer(customer, accountNumber, type);

            //Assert
            Assert.That(result.IsSuccess, Is.True);
            _accountRepositoryMock.Verify(
                repo => repo.Add(It.Is<Account>(a =>
                    a.AccountNumber == accountNumber &&
                    a.AccountType == type &&
                    a.CustomerId == customer.Id)),
                Times.Once,
                "The 'Add' method of the injected repository is not called or the passed in Account does not have the correct property values");
        }

        [MonitoredTest("AccountService - TransferMoney - One of the accounts does not exist - Should throw InvalidOperationException")]
        public void TransferMoney_OneOfTheAccountsDoesNotExist_ShouldThrowInvalidOperationException()
        {
            //Arrange
            bool fromIsNull = Random.Next(0, 2) == 0;
            string fromAccountNumber = Guid.NewGuid().ToString();
            string toAccountNumber = Guid.NewGuid().ToString();

            if (!fromIsNull)
            {
                _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(fromAccountNumber)).Returns(new AccountBuilder().Build());
                _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(toAccountNumber)).Returns(() => null);
            }
            else
            {
                _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(fromAccountNumber)).Returns(() => null);
                _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(toAccountNumber)).Returns(new AccountBuilder().Build());
            }


            //Act + Assert
            Assert.That(() => _service.TransferMoney(fromAccountNumber, toAccountNumber, 1), Throws.InvalidOperationException);
            _accountRepositoryMock.Verify(repo => repo.CommitChanges(), Times.Never,
                "The 'CommitChanges' method of the repository should not have been called.");

        }

        [MonitoredTest("AccountService - TransferMoney - Should adjust balances and save the changes")]
        public void TransferMoney_ShouldAdjustBalancesAndSaveTheChanges()
        {
            //Arrange
            decimal originalFromBalance = Random.Next(500, 1001);
            decimal originalToBalance = Random.Next(0, 1001);
            Account fromAccount = new AccountBuilder().WithBalance(originalFromBalance).Build();
            Account toAccount = new AccountBuilder().WithBalance(originalToBalance).Build();

            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(fromAccount.AccountNumber)).Returns(fromAccount);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(toAccount.AccountNumber)).Returns(toAccount);

            decimal amount = Random.Next(10, 101);

            //Act
            Result result = _service.TransferMoney(fromAccount.AccountNumber, toAccount.AccountNumber, amount);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A 'success' result should be returned.");
            Assert.That(fromAccount.Balance, Is.EqualTo(originalFromBalance - amount),
                "The balance of the 'from' account is not correct after the transaction.");
            Assert.That(toAccount.Balance, Is.EqualTo(originalToBalance + amount),
                "The balance of the 'to' account is not correct after the transaction.");
            _accountRepositoryMock.Verify(repo => repo.GetByAccountNumber(fromAccount.AccountNumber), Times.Once,
                "The 'GetByAccountNumber' method of the repository should have been called once for the 'from' account number.");
            _accountRepositoryMock.Verify(repo => repo.GetByAccountNumber(toAccount.AccountNumber), Times.Once,
                "The 'GetByAccountNumber' method of the repository should have been called once for the 'to' account number.");
            _accountRepositoryMock.Verify(repo => repo.CommitChanges(), Times.Once,
                "The 'CommitChanges' method of the repository should have been called.");

        }

        [MonitoredTest("AccountService - TransferMoney - Insufficient funds and no youth account - Should allow the balance to be negative")]
        public void TransferMoney_InsufficientFundsAndNoYouthAccount_ShouldAllowTheBalanceToBeNegative()
        {
            //Arrange
            decimal originalFromBalance = Random.Next(10, 101);
            decimal originalToBalance = Random.Next(0, 1001);

            AccountType type = Random.NextAccountType();
            while (type == AccountType.YouthAccount)
            {
                type = Random.NextAccountType();
            }

            Account fromAccount = new AccountBuilder().WithBalance(originalFromBalance).WithType(type).Build();
            Account toAccount = new AccountBuilder().WithBalance(originalToBalance).Build();

            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(fromAccount.AccountNumber)).Returns(fromAccount);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(toAccount.AccountNumber)).Returns(toAccount);

            decimal amount = Random.Next(200, 1001);

            //Act
            Result result = _service.TransferMoney(fromAccount.AccountNumber, toAccount.AccountNumber, amount);

            //Assert
            Assert.That(result.IsSuccess, Is.True, "A 'success' result should be returned.");
            Assert.That(fromAccount.Balance, Is.EqualTo(originalFromBalance - amount),
                "The balance of the 'from' account is not correct after the transaction.");
            _accountRepositoryMock.Verify(repo => repo.CommitChanges(), Times.Once,
                "The 'CommitChanges' method of the repository should have been called.");

        }

        [MonitoredTest("AccountService - TransferMoney - Insufficient funds in youth account - Should return failure")]
        public void TransferMoney_InsufficientFundsInYouthAccount_ShouldReturnFailure()
        {
            //Arrange
            decimal originalFromBalance = Random.Next(10, 101);
            decimal originalToBalance = Random.Next(0, 1001);

            Account fromAccount = new AccountBuilder().WithBalance(originalFromBalance).WithType(AccountType.YouthAccount).Build();
            Account toAccount = new AccountBuilder().WithBalance(originalToBalance).Build();

            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(fromAccount.AccountNumber)).Returns(fromAccount);
            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumber(toAccount.AccountNumber)).Returns(toAccount);

            decimal amount = Random.Next(200, 1001);

            //Act
            Result result = _service.TransferMoney(fromAccount.AccountNumber, toAccount.AccountNumber, amount);

            //Assert
            Assert.That(result.IsSuccess, Is.False, "A 'failure' result should be returned.");
            _accountRepositoryMock.Verify(repo => repo.CommitChanges(), Times.Never,
                "The 'CommitChanges' method of the repository should not have been called.");

        }
    }
}