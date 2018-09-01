using System;
using Bank.Data.DomainClasses;

namespace Bank.Tests
{
    internal class CustomerBuilder
    {
        private readonly Customer _customer;
        private readonly Random _random;

        public CustomerBuilder()
        {
            _random = new Random();
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
            _customer.CustomerId = id;
            return this;
        }

        public CustomerBuilder WithZipCode(int zipCode)
        {
            _customer.ZipCode = zipCode;
            return this;
        }

        public CustomerBuilder WithNullProperties()
        {
            _customer.FirstName = null;
            _customer.Name = null;
            _customer.Address = null;
            _customer.CellPhone = null;
            return this;
        }

        public Customer Build()
        {
            return _customer;
        }
    }
}