using System.Collections.Generic;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.UI
{
    public class WindowDialogService : IWindowDialogService
    {
        private readonly IAccountRepository _accountRepository;

        public WindowDialogService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public bool? ShowAccountDialogForCustomer(Customer customer)
        {
            var accountsWindow = new AccountsWindow(customer, _accountRepository, this);
            return accountsWindow.ShowDialog();
        }

        public bool? ShowTransferDialog(Account fromAccount, IList<Account> allAccountsOfCustomer)
        {
            var transferWindow = new TransferWindow(fromAccount, allAccountsOfCustomer, _accountRepository);
            return transferWindow.ShowDialog();
        }
    }
}