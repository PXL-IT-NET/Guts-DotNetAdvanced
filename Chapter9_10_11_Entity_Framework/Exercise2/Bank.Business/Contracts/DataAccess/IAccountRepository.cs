using Bank.Domain;

namespace Bank.Business.Contracts.DataAccess
{
    public interface IAccountRepository
    {
        void Add(Account newAccount);
        void Update(Account existingAccount);
        void TransferMoney(int fromAccountId, int toAccountId, decimal amount);
    }
}