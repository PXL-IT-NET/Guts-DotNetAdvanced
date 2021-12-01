using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.Domain\Account.cs;")]
    public class AccountTests
    {
        private static Random Random = new Random();

        [MonitoredTest("Account - CreateNewForCustomer - Should set initial 'Balance' to 100")]
        public void CreateNewForCustomer_ShouldSetInitialBalanceTo100()
        {
            //Arrange
            int customerId = Random.Next(1, int.MaxValue);
            string accountNumber = Guid.NewGuid().ToString();

            //Act
            Account account = Account.CreateNewForCustomer(customerId, accountNumber, AccountType.PaymentAccount);

            //Assert
            Assert.That(account.Balance, Is.EqualTo(100));
        }

        [MonitoredTest("Account - CreateNewForCustomer - Empty account - Should throw ArgumentException")]
        public void CreateNewForCustomer_EmptyAccount_ShouldThrowArgumentException()
        {
            //Arrange
            int customerId = Random.Next(1, int.MaxValue);
            string emptyAccountNumber = string.Empty;

            //Act + Assert
            Assert.That(() => Account.CreateNewForCustomer(customerId, emptyAccountNumber, AccountType.PaymentAccount), Throws.ArgumentException);
        }

        [MonitoredTest("Account - CreateNewForCustomer - ZeroOrNegativeCustomerId - Should throw ArgumentException")]
        public void CreateNewForCustomer_ZeroOrNegativeCustomerId_ShouldThrowArgumentException()
        {
            //Arrange
            int invalidCustomerId = 1 - Random.Next(1, int.MaxValue);
            string accountNumber = Guid.NewGuid().ToString();

            //Act + Assert
            Assert.That(() => Account.CreateNewForCustomer(invalidCustomerId, accountNumber, AccountType.PaymentAccount), Throws.ArgumentException);
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

        [MonitoredTest("Account - Should make the default constructor private so that only 'Entity Framework' can use it")]
        public void ShouldMakeTheDefaultConstructorPrivateSoThatOnlyEntityFrameworkCanUseIt()
        {
            var accountType = typeof(Account);
            List<ConstructorInfo> privateConstructors = accountType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(c => c.IsPrivate).ToList();

            Assert.That(privateConstructors, Has.Count.EqualTo(1),
                "There should be exactly 1 private constructor. " +
                "By doing this other classes (layers) will be forced to use the 'CreateNewForCustomer' method to create an account.");

            ConstructorInfo constructor = privateConstructors.First();
            Assert.That(constructor.GetParameters().Length, Is.Zero, "The private constructor should not have any parameters. It does not need to do anything.");
        }
    }
}
