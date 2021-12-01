using System;
using System.Windows;
using System.Windows.Controls;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

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

        private void ShowAccountsButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)e.Source;
            Customer selectedCustomer = (Customer)clickedButton.Tag;

            //TODO: use dialog service to show transfer dialog
        }
    }
}
