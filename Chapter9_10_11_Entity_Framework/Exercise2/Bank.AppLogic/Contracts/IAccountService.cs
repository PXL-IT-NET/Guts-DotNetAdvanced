using Bank.Domain;

namespace Bank.AppLogic.Contracts
{
    public interface IAccountService
    {
        Result AddNewAccountForCustomer(Customer customer, string accountNumber, AccountType type);
        Result TransferMoney(string fromAccountNumber, string toAccountNumber, decimal amount);
    }
}