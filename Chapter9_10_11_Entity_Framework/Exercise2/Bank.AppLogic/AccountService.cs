using System;
using Bank.AppLogic.Contracts;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.AppLogic
{
    internal class AccountService : IAccountService
    {
        public AccountService(IAccountRepository accountRepository)
        {
        }

        public Result AddNewAccountForCustomer(Customer customer, string accountNumber, AccountType type)
        {
            throw new NotImplementedException();
        }

        public Result TransferMoney(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}