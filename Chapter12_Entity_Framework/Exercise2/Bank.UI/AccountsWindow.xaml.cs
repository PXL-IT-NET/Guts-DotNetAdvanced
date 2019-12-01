using System;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;
using System.Windows;
using Bank.Business.Interfaces;

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
