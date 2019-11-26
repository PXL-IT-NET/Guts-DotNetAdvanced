using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Data
{
    public class CustomerRepository : ICustomerRepository
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