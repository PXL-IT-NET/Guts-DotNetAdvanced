using System;
using System.Collections.Generic;
using System.Windows;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.UI
{
    public partial class TransferWindow : Window
    {
        public TransferWindow(Account fromAccount,
            IList<Account> allAccountsOfCustomer,
            IAccountRepository accountRepository)
        {
            InitializeComponent();
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
