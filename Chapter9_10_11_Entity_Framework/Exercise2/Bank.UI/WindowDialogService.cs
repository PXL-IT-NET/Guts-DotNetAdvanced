using System.Collections.Generic;
using Bank.Business.Contracts;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;
using Bank.UI;

public class WindowDialogService : IWindowDialogService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountValidator _accountValidator;

    public WindowDialogService(IAccountRepository accountRepository, IAccountValidator accountValidator)
    {
        _accountRepository = accountRepository;
        _accountValidator = accountValidator;
    }

    public bool? ShowAccountDialogForCustomer(Customer customer)
    {
        var accountsWindow = new AccountsWindow(customer, _accountRepository, _accountValidator, this);
        return accountsWindow.ShowDialog();
    }

    public bool? ShowTransferDialog(Account fromAccount, IEnumerable<Account> allAccountsOfCustomer)
    {
        var transferWindow = new TransferWindow(fromAccount, allAccountsOfCustomer, _accountRepository);
        return transferWindow.ShowDialog();
    }
}