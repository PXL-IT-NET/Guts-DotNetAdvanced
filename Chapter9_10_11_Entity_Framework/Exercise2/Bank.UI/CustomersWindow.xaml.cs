using System;
using System.Windows;
using Bank.Business.Contracts;
using Bank.Business.Contracts.DataAccess;

namespace Bank.UI
{
    public partial class CustomersWindow : Window
    {
        public CustomersWindow(ICustomerRepository customerRepository,
            ICustomerValidator customerValidator,
            ICityRepository cityRepository,
            IWindowDialogService windowDialogService)
        {
            InitializeComponent();
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SaveCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ShowAccountsButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}