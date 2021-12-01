using System;
using System.Linq;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure
{
    internal class AccountRepository : IAccountRepository
    {
        public AccountRepository(BankContext context)
        {
        }

        public Account GetByAccountNumber(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public void Add(Account newAccount)
        {
            throw new NotImplementedException();
        }

        public void CommitChanges()
        {
            throw new NotImplementedException();
        }
    }
}
