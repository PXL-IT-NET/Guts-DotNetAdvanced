using System.Collections.Generic;
using Bank.AppLogic.Contracts;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.UI
{
    public class WindowDialogService : IWindowDialogService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountService _accountService;

        public WindowDialogService(IAccountRepository accountRepository, IAccountService accountService)
        {
            _accountRepository = accountRepository;
            _accountService = accountService;
        }

        public bool? ShowAccountDialogForCustomer(Customer customer)
        {
            var accountsWindow = new AccountsWindow(customer, _accountService, this);
            return accountsWindow.ShowDialog();
        }

        public bool? ShowTransferDialog(Account fromAccount, IEnumerable<Account> allAccountsOfCustomer)
        {
            var transferWindow = new TransferWindow(fromAccount, allAccountsOfCustomer, _accountService);
            return transferWindow.ShowDialog();
        }
    }
}