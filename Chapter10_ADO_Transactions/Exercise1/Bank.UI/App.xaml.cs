using System.Windows;
using Bank.Data;

namespace Bank.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var connectionFactory = new ConnectionFactory();
            var customerRepository = new CustomerRepository(connectionFactory);
            var cityRepository = new CityRepository(connectionFactory);
            var accountRepository = new AccountRepository(connectionFactory);
            var dialogService = new WindowDialogService(accountRepository);

            var customersWindow = new CustomersWindow(customerRepository, cityRepository, dialogService);
            customersWindow.Show();
        }
    }
}
