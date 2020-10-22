using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Business;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;
using Bank.Domain.Enums;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise02",
        @"Bank.Business\AccountValidator.cs")]
    internal class AccountValidatorTests
    {
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private AccountValidator _validator;
        private readonly Random _random;
        private List<Customer> _existingCustomers;

        public AccountValidatorTests()
        {
            _random = new Random();
        }

        [SetUp]
        public void SetUp()
        {
            _existingCustomers = new List<Customer>()
            {
                new CustomerBuilder(_random).WithId().Build(),
                new CustomerBuilder(_random).WithId().Build(),
                new CustomerBuilder(_random).WithId().Build()
            };
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerRepositoryMock.Setup(repo => repo.GetAllWithAccounts()).Returns(_existingCustomers);
            _validator = new AccountValidator(_customerRepositoryMock.Object);
        }

        [MonitoredTest("AccountValidator - IsValid should pass for valid account")]
        public void IsValid_ShouldPassForValidAccount()
        {
            //Arrange
            var account = new AccountBuilder().WithCustomerId(_existingCustomers.First().Id).Build();

            //Act
            var result = _validator.IsValid(account);

            //Assert
            Assert.That(result.IsValid, Is.True);
        }

        [MonitoredTest("AccountValidator - IsValid should fail when account is null")]
        public void IsValid_ShouldFailWhenAccountIsNull()
        {
            //Act
            var result = _validator.IsValid(null);

            //Assert
            Assert.That(result.IsValid, Is.False, "Result should be invalid.");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty, "Message should not be empty.");
        }

        [MonitoredTest("AccountValidator - IsValid should fail on invalid properties")]
        public void IsValid_ShouldFailOnInvalidProperties()
        {
            AssertIsInvalid(1, "", 100, AccountType.PaymentAccount);
            AssertIsInvalid(1, null, 100, AccountType.PaymentAccount);
            AssertIsInvalid(0, "validAccountNumber", -10, AccountType.PaymentAccount);
            AssertIsInvalid(1, "validAccountNumber", 100, (AccountType)(-1));
        }

        [MonitoredTest("AccountValidator - IsValid should fail on non existing customer")]
        public void IsValid_ShouldFailOnNonExistingCustomer()
        {
            //Arrange
            var nonExistingCustomerId = _random.Next();
            while (_existingCustomers.Any(c => c.Id == nonExistingCustomerId))
            {
                nonExistingCustomerId = _random.Next();
            }

            var account = new AccountBuilder().WithCustomerId(nonExistingCustomerId).Build();

            //Act
            var result = _validator.IsValid(account);

            //Assert
            Assert.That(result.IsValid, Is.False, "Result should be invalid.");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty, "Message should not be empty.");
        }

        private void AssertIsInvalid(int id, string accountNumber, decimal balance, AccountType accountType)
        {
            //Arrange
            var account = new AccountBuilder()
                .WithId(id)
                .WithAccountNumber(accountNumber)
                .WithBalance(balance)
                .WithCustomerId(_existingCustomers.First().Id).Build();
            account.AccountType = accountType;

            //Act
            var result = _validator.IsValid(account);

            //Assert
            var forMessage =
                $"for account with id '{id}', account number '{accountNumber ?? "null"}', " +
                $"balance '{balance}' and account type '{accountType}'.";
            Assert.That(result.IsValid, Is.False,
                $"Result should be invalid {forMessage}");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty,
                $"Message should not be empty {forMessage}");
        }
    }
}