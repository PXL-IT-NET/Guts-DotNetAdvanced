using System.Windows;
using Bank.Business;
using Bank.Data;

namespace Bank.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = new BankContext();
            context.CreateOrUpdateDatabase();

            var customerRepository = new CustomerRepository(context);
            var cityRepository = new CityRepository(context);
            var accountRepository = new AccountRepository(context);

            var customerValidator = new CustomerValidator(cityRepository);
            var accountValidator = new AccountValidator(customerRepository);

            var dialogService = new WindowDialogService(accountRepository, accountValidator);

            var customersWindow = new CustomersWindow(customerRepository, customerValidator, cityRepository, dialogService);
            customersWindow.Show();
        }
    }
}
