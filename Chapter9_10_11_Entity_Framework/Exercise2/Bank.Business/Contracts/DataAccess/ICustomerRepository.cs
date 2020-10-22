using System.Collections.Generic;
using Bank.Domain;

namespace Bank.Business.Contracts.DataAccess
{
    public interface ICustomerRepository
    {
        IList<Customer> GetAllWithAccounts();
        void Update(Customer existingCustomer);
        void Add(Customer newCustomer);
    }
}