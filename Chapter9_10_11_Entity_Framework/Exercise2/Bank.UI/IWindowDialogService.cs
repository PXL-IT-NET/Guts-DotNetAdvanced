using System.Collections.Generic;
using Bank.Data.DomainClasses;

namespace Bank.UI
{
    public interface IWindowDialogService
    {
        bool? ShowAccountDialogForCustomer(Customer customer);
        bool? ShowTransferDialog(Account fromAccount, IEnumerable<Account> allAccountsOfCustomer);
    }
}