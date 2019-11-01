using System;
using System.Windows;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.UI
{
    public partial class CustomersWindow : Window
    {
        public CustomersWindow(ICustomerRepository customerRepository,
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
