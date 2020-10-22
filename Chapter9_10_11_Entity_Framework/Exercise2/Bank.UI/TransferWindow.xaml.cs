using System;
using System.Collections.Generic;
using System.Windows;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.UI
{
    public partial class TransferWindow : Window
    {
        public TransferWindow(Account fromAccount,
            IEnumerable<Account> allAccountsOfCustomer,
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