using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Data
{
    public class AccountRepository : IAccountRepository
    {
        public AccountRepository(BankContext context)
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
    }
}
