using NUnit.Framework;
using System.ComponentModel;
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise01",
        @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    public class AccountTests
    {
        [MonitoredTest("Account - Should implement INotifyPropertyChanged and use it for the 'Balance' property")]
        public void ShouldImplementINotifyPropertyChangedAndUseItForBalance()
        {
            var account = new AccountBuilder().Build();
            INotifyPropertyChanged notifier = account as INotifyPropertyChanged;
            Assert.That(notifier, Is.Not.Null, () => "INotifyPropertyChanged is not implemented.");

            var notifyForBalancePropertyReceived = false;
            notifier.PropertyChanged += (sender, e) =>
            {
                notifyForBalancePropertyReceived = e.PropertyName == "Balance";
            };

            account.Balance += 1;
            Assert.That(notifyForBalancePropertyReceived, Is.True, () => "No 'PropertyChanged' event it triggerd when the 'Balance' property changes.");
        }

    }
}
