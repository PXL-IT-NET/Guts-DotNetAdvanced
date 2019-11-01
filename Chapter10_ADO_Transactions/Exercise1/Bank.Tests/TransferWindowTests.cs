using System;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;
using Bank.UI;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared.TestTools;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise01",
         @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class TransferWindowTests
    {
        private TransferWindow _window;
        private Button _transferButton;
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Account _fromAccount;
        private Account _toAccount;
        private List<Account> _allAccountsOfCustomer;
        private ComboBox _toAccountsComboBox;
        private TextBlock _fromAccountTextBlock;
        private TextBox _amountTextBox;
        private TextBlock _errorMessageTextBlock;

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _fromAccount = new AccountBuilder().WithId().Build();
            _toAccount = new AccountBuilder().WithId().Build();
            _allAccountsOfCustomer = new List<Account> { _fromAccount, _toAccount };

            _window = new TransferWindow(_fromAccount, _allAccountsOfCustomer, _accountRepositoryMock.Object);
            _window.Show();

            _fromAccountTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("FromAccountTextBlock");
            _toAccountsComboBox = _window.FindVisualChildren<ComboBox>().First();
            _amountTextBox = _window.GetPrivateFieldValueByName<TextBox>("AmountTextBox");
            _errorMessageTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("ErrorMessageTextBlock");
            _transferButton = _window.GetPrivateFieldValueByName<Button>("TransferButton");
        }

        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("TransferWindow - Should initialize controls on construction"), Order(1)]
        public void _1_ShouldInitializeControlsOnConstruction()
        {
            //Arrange + Act -> SetUp

            //Assert
            Assert.That(_fromAccountTextBlock.Text, Is.EqualTo(_fromAccount.AccountNumber),
                () => "The 'FromAccountTextBlock' should show the account number of the 'From' account");
            Assert.That(_toAccountsComboBox.ItemsSource, Is.EqualTo(_allAccountsOfCustomer),
                () => "The 'ItemsSource' property of the combobox should be the list of accounts of the customer " +
                      "that is passed in the constructor as a parameter");
            Assert.That(_errorMessageTextBlock.Visibility, Is.EqualTo(Visibility.Hidden),
                () => "The 'Visibility' of the 'ErrorMessageTextBlock' should be set to hidden");
        }

        [MonitoredTest("TransferWindow - Should tranfer a valid amount"), Order(2)]
        public void _2_ShouldTransferAValidAmount()
        {
            //Arrange
            var orginalFromBalance = _fromAccount.Balance;
            var orignalToBalance = _toAccount.Balance;
            decimal amount = Convert.ToDecimal(new Random().NextDouble()) * _fromAccount.Balance;
            _amountTextBox.Text = Convert.ToString(amount);

            SelectTheToAccountInTheComboBox();

            //Act
            _transferButton.FireClickEvent();

            //Assert
            _accountRepositoryMock.Verify(repo => repo.TransferMoney(_fromAccount.Id, _toAccount.Id, amount),
                Times.Once, "The 'TransferMony' method of the repository is not called correctly.");
            Assert.That(_fromAccount.Balance, Is.EqualTo(orginalFromBalance - amount),
                () =>
                    "Next to transfering the money in the database using the 'TransferMoney' method, " +
                    "you should also update the balance of the 'from account' object so that the changes are reflected in the UI.");
            Assert.That(_toAccount.Balance, Is.EqualTo(orignalToBalance + amount), () =>
            "Next to transfering the money in the database using the 'TransferMoney' method, " +
                "you should also update the balance of the 'to account' object so that the changes are reflected in the UI.");
        }

        [MonitoredTest("TransferWindow - Should show an error message when the amount is out of range"), Order(3)]
        public void _3_ShouldShowErrorMessageWhenAmountIsOutOfRange()
        {
            //Arrange
            decimal amount = Convert.ToDecimal(_fromAccount.Balance + 1);
            TestAmountThatIsOutOfRange(amount);
            TestAmountThatIsOutOfRange(-1);
        }

        private void TestAmountThatIsOutOfRange(decimal amount)
        {
            //Arrange
            _amountTextBox.Text = Convert.ToString(amount);

            SelectTheToAccountInTheComboBox();

            //Act
            _transferButton.FireClickEvent();

            //Assert
            _accountRepositoryMock.Verify(
                repo => repo.TransferMoney(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never,
                $"TransferMoney should never be called when amount ({amount}) is out of range.");
            Assert.That(_errorMessageTextBlock.Visibility, Is.EqualTo(Visibility.Visible),
                () => "The 'Visibility' of the error message should be visible.");
            Assert.That(_errorMessageTextBlock.Text, Contains.Substring(Convert.ToString(_fromAccount.Balance)),
                () => "The error message should contain the maximum amount allowed.");
        }

        private void SelectTheToAccountInTheComboBox()
        {
            Assert.That(_toAccountsComboBox.Items.Count, Is.EqualTo(_allAccountsOfCustomer.Count),
                () => "The combobox should have an item for each account of the customer.");
            _toAccountsComboBox.SelectedIndex = 1; //select toAccount
        }
    }
}
