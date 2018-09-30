using System;
using Bank.Data.DomainClasses;
using Bank.Data.DomainClasses.Enums;

namespace Bank.Tests
{
    internal class AccountBuilder
    {
        private readonly Account _account;
        private readonly Random _random;

        public AccountBuilder()
        {
            _random = new Random();
            var accountTypeValues = Enum.GetValues(typeof(AccountType));

            _account = new Account
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AccountType = (AccountType)accountTypeValues.GetValue(_random.Next(accountTypeValues.Length)),
                Balance = _random.Next(0, int.MaxValue),
                CustomerId = 0
            };
        }

        public AccountBuilder WithCustomerId()
        {
            return WithCustomerId(_random.Next(1, int.MaxValue));
        }

        public AccountBuilder WithCustomerId(int customerId)
        {
            _account.CustomerId = customerId;
            return this;
        }

        public AccountBuilder WithId()
        {
            return WithId(_random.Next(1, int.MaxValue));
        }

        public AccountBuilder WithId(int id)
        {
            _account.Id = id;
            return this;
        }

        public AccountBuilder WithAccountNumber(string value)
        {
            _account.AccountNumber = value;
            return this;
        }

        public Account Build()
        {
            return _account;
        }
    }
}