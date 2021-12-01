using System;
using System.Collections.Generic;
using System.Windows;
using Bank.AppLogic.Contracts;
using Bank.Domain;

namespace Bank.UI
{
    public partial class TransferWindow : Window
    {
        private readonly Account _fromAccount;
        private readonly IAccountService _accountService;

        public TransferWindow(Account fromAccount, 
            IEnumerable<Account> allAccountsOfCustomer,
            IAccountService accountService)
        {
            InitializeComponent();

            _fromAccount = fromAccount;
            _accountService = accountService;

            FromAccountTextBlock.Text = _fromAccount.AccountNumber;
            ToAccountComboBox.ItemsSource = allAccountsOfCustomer;
            ErrorMessageTextBlock.Visibility = Visibility.Hidden;
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            decimal amount = Convert.ToDecimal(AmountTextBox.Text);
            Account toAccount = (Account)ToAccountComboBox.SelectedItem;
            Result result = _accountService.TransferMoney(_fromAccount.AccountNumber, toAccount.AccountNumber, amount);

            if (result.IsSuccess)
            {
                Close();
            }
            else
            {
                ErrorMessageTextBlock.Text = result.Message;
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
