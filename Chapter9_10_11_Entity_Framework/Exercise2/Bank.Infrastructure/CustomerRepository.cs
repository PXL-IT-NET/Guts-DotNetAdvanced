using System;
using System.Collections.Generic;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Infrastructure
{
    internal class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository(BankContext context)
        {
        }

        public IReadOnlyList<Customer> GetAllWithAccounts()
        {
            throw new NotImplementedException();
        }

        public void Add(Customer newCustomer)
        {
            throw new NotImplementedException();
        }
    }
}