using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository(IConnectionFactory connectionFactory)
        {

        }

        public IList<Customer> GetAll()
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