using System.Collections.Generic;
using Bank.Data.DomainClasses;

namespace Bank.Data.Interfaces
{
    public interface IAccountRepository
    {
        void Add(Account newAccount);
        void Update(Account existingAccount);
        void TransferMoney(int fromAccountId, int toAccountId, decimal amount);
    }
}