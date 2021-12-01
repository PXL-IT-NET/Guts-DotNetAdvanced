using System;
using System.Collections.Generic;
using Bank.Domain;

namespace Bank.Tests
{
    internal class CustomerBuilder
    {
        private readonly Customer _customer;
        private static readonly Random Random = new Random();

        public CustomerBuilder()
        {
            _customer = new Customer
            {
                FirstName = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Address = Guid.NewGuid().ToString(),
                ZipCode = Random.Next(1000,10000)
            };
            _customer.TrySetAccounts(new List<Account>());
        }

        public CustomerBuilder WithId()
        {
            return WithId(Random.Next(1, int.MaxValue));
        }

        public CustomerBuilder WithId(int id)
        {
            _customer.Id = id;
            return this;
        }

        public CustomerBuilder WithAccounts()
        {
            var accounts = new List<Account>();
            for (int i = 0; i < Random.Next(1, 6); i++)
            {
                accounts.Add(new AccountBuilder().WithCustomerId(_customer.Id).Build());
            }

            _customer.TrySetAccounts(accounts);
            return this;
        }

        public CustomerBuilder WithName(string name)
        {
            _customer.Name = name;
            return this;
        }

        public CustomerBuilder WithFirstName(string firstName)
        {
            _customer.FirstName = firstName;
            return this;
        }

        public CustomerBuilder WithAddress(string address)
        {
            _customer.Address = address;
            return this;
        }

        public CustomerBuilder WithZipCode(int zipCode)
        {
            _customer.ZipCode = zipCode;
            return this;
        }

        public Customer Build()
        {
            return _customer;
        }
    }
}