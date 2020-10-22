using System;
using System.Windows;
using Bank.Business.Contracts;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.UI
{
    public partial class AccountsWindow : Window
    {
        public AccountsWindow(Customer customer,
            IAccountRepository accountRepository,
            IAccountValidator accountValidator,
            IWindowDialogService windowDialogService)
        {
            InitializeComponent();
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SaveAccountButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}