using System.Collections.Generic;
using Bank.Domain;

namespace Bank.AppLogic.Contracts.DataAccess
{
    public interface ICustomerRepository
    {
        IReadOnlyList<Customer> GetAllWithAccounts();
        void Add(Customer newCustomer);
    }
}