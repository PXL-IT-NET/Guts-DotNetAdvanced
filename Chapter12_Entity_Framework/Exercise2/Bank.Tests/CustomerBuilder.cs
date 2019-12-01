using System;
using System.Collections.Generic;
using Bank.Data.DomainClasses;

namespace Bank.Tests
{
    internal class CustomerBuilder
    {
        private readonly Customer _customer;
        private readonly Random _random;

        public CustomerBuilder(Random random = null)
        {
            _random = random ?? new Random();
            _customer = new Customer
            {
                FirstName = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Address = Guid.NewGuid().ToString(),
                CellPhone = Guid.NewGuid().ToString(),
                ZipCode = _random.Next(1, int.MaxValue)
            };
        }


        public CustomerBuilder WithId()
        {
            return WithId(_random.Next(1, int.MaxValue));
        }

        public CustomerBuilder WithId(int id)
        {
            _customer.Id = id;
            return this;
        }

        public CustomerBuilder WithZipCode(int zipCode)
        {
            _customer.ZipCode = zipCode;
            return this;
        }

        public CustomerBuilder WithAccounts()
        {
            var accounts = new List<Account>();
            for (int i = 0; i < _random.Next(1, 6); i++)
            {
                accounts.Add(new AccountBuilder().WithCustomerId(_customer.Id).Build());
            }

            _customer.TrySetAccounts(accounts);
            return this;
        }

        public CustomerBuilder WithAccounts(ICollection<Account> accounts)
        {
            _customer.TrySetAccounts(accounts);
            return this;
        }

        public Customer Build()
        {
            return _customer;
        }
    }
}