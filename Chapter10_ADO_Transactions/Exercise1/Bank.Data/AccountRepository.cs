using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bank.Data.DomainClasses;
using Bank.Data.DomainClasses.Enums;
using Bank.Data.Interfaces;

namespace Bank.Data
{
    public class AccountRepository : IAccountRepository
    {
        public AccountRepository(IConnectionFactory connectionFactory)
        {

        }

        public void Add(Account newAccount)
        {
            throw new NotImplementedException();
        }

        public void Update(Account existingAccount)
        {
            throw new NotImplementedException();
        }

        public void TransferMoney(int fromAccountId, int toAccountId, decimal amount)
        {
            throw new NotImplementedException();
        }

        public IList<Account> GetAllAccountsOfCustomer(int customerId)
        {
            throw new NotImplementedException();
        }
    }
}
