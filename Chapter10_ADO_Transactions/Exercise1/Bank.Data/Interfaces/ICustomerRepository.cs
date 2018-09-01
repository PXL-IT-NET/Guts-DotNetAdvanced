using System.Collections.Generic;
using Bank.Data.DomainClasses;

namespace Bank.Data.Interfaces
{
    public interface ICustomerRepository
    {
        IList<Customer> GetAll();
        void Update(Customer existingCustomer);
        void Add(Customer newCustomer);
    }
}