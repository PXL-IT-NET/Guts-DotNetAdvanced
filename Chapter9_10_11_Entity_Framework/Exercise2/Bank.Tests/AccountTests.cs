using NUnit.Framework;
using System.ComponentModel;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.Domain\Account.cs;")]
    public class AccountTests
    {
        [MonitoredTest("Account - Constructor - Should set initial 'Balance' to 1000")]
        public void Constructor_ShouldSetInitialBalanceTo1000()
        {
            var account = new Account();
            Assert.That(account.Balance, Is.EqualTo(1000));
        }

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
