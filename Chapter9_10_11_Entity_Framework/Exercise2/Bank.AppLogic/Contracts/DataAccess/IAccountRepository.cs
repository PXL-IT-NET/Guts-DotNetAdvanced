using Bank.Domain;

namespace Bank.AppLogic.Contracts.DataAccess
{
    public interface IAccountRepository
    {
        Account GetByAccountNumber(string accountNumber);
        void Add(Account newAccount);
        void CommitChanges();
    }
}