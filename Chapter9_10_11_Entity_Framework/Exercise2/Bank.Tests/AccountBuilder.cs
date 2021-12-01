using System;
using Bank.Domain;

namespace Bank.Tests
{
    internal class AccountBuilder
    {
        private readonly Account _account;
        private readonly Random _random;

        public AccountBuilder()
        {
            _random = new Random();
            
            string accountNumber = Guid.NewGuid().ToString();
            AccountType type = _random.NextAccountType();
            int customerId = _random.Next(1, int.MaxValue);

            _account = Account.CreateNewForCustomer(customerId, accountNumber, type);
            _account.Balance = _random.Next(0, 1000000);
        }

        public AccountBuilder WithCustomerId(int customerId)
        {
            _account.CustomerId = customerId;
            return this;
        }

        public AccountBuilder WithType(AccountType type)
        {
            _account.AccountType = type;
            return this;
        }

        public AccountBuilder WithBalance(decimal balance)
        {
            _account.Balance = balance;
            return this;
        }

        public Account Build()
        {
            return _account;
        }

       
    }
}