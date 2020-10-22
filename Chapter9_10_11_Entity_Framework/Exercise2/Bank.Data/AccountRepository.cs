using System;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Data
{
    internal class AccountRepository : IAccountRepository
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
