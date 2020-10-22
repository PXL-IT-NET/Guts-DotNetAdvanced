using System.Collections.Generic;
using Bank.Domain;

namespace Bank.UI
{
    public interface IWindowDialogService
    {
        bool? ShowAccountDialogForCustomer(Customer customer);
        bool? ShowTransferDialog(Account fromAccount, IEnumerable<Account> allAccountsOfCustomer);
    }
}