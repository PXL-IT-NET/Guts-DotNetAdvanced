using System;
using System.Collections.Generic;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Data
{
    internal class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository(BankContext context)
        {
        }

        public IList<Customer> GetAllWithAccounts()
        {
            throw new NotImplementedException();
        }

        public void Update(Customer existingCustomer)
        {
            throw new NotImplementedException();
        }

        public void Add(Customer newCustomer)
        {
            throw new NotImplementedException();
        }
    }
}