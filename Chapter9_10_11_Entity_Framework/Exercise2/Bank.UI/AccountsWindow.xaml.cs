using System;
using System.Windows;
using System.Windows.Controls;
using Bank.AppLogic;
using Bank.AppLogic.Contracts;
using Bank.Domain;

namespace Bank.UI
{
    public partial class AccountsWindow : Window
    {
        public AccountsWindow(Customer customer,
            IAccountService accountService,
            IWindowDialogService windowDialogService)
        {
            InitializeComponent();
            TypeComboBox.SelectedIndex = 0;
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)e.Source;
            Account selectedAccount = (Account)clickedButton.Tag;

            //TODO: use dialog service to show transfer dialog
        }
    }
}
