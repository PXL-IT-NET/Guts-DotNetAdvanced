using System;
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
using Bank.AppLogic.Contracts;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class TransferWindowTests
    {
        private TransferWindow _window;
        private Button _transferButton;
        private Mock<IAccountService> _accountServiceMock;
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
            _accountServiceMock = new Mock<IAccountService>();
        }

        [TearDown]
        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("TransferWindow - Should initialize controls on construction"), Order(1)]
        public void _1_ShouldInitializeControlsOnConstruction()
        {
            //Arrange + Act
            InitializeWindow();

            //Assert
            Assert.That(_fromAccountTextBlock.Text, Is.EqualTo(_fromAccount.AccountNumber),
                () => "The 'FromAccountTextBlock' should show the account number of the 'From' account");
            Assert.That(_toAccountsComboBox.ItemsSource, Is.EqualTo(_allAccountsOfCustomer),
                () => "The 'ItemsSource' property of the combobox should be the list of accounts of the customer " +
                      "that is passed in the constructor as a parameter");
            Assert.That(_errorMessageTextBlock.Visibility, Is.EqualTo(Visibility.Hidden),
                () => "The 'Visibility' of the 'ErrorMessageTextBlock' should be set to hidden");
        }

        [MonitoredTest("TransferWindow - TransferButton click - Valid transaction - Should use service to transfer amount and close itself"), Order(2)]
        public void _2_TransferButtonClick_ValidTransaction_ShouldUseServiceToTransferAmountAndCloseItself()
        {
            //Arrange
            InitializeWindow();

            decimal amount = Convert.ToDecimal(new Random().NextDouble()) * _fromAccount.Balance;
            _amountTextBox.Text = Convert.ToString(amount);

            SelectTheToAccountInTheComboBox();

            Result successResult = Result.Success();
            _accountServiceMock
                .Setup(service => service.TransferMoney(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns(successResult);

            //Act
            _transferButton.FireClickEvent();

            //Assert
            _accountServiceMock.Verify(service => service.TransferMoney(_fromAccount.AccountNumber, _toAccount.AccountNumber, amount),
                Times.Once, "The 'TransferMoney' method of the repository is not called correctly.");

            Assert.That(_window.IsVisible || _window.IsActive, Is.False, "The window should close itself after a successful transaction.");
        }

        [MonitoredTest("TransferWindow - TransferButton click - Should show an error message when the transfer fails"), Order(3)]
        public void _3_TransferButtonClick_ShouldShowErrorMessageWhenTransferFails()
        {
            //Arrange
            InitializeWindow();

            decimal amount = 1.1m * _fromAccount.Balance;
            _amountTextBox.Text = Convert.ToString(amount);

            SelectTheToAccountInTheComboBox();

            Result failureResult = Result.Fail(Guid.NewGuid().ToString());
            _accountServiceMock
                .Setup(service => service.TransferMoney(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns(failureResult);

            //Act
            _transferButton.FireClickEvent();

            //Assert
            _accountServiceMock.Verify(service => service.TransferMoney(_fromAccount.AccountNumber, _toAccount.AccountNumber, amount),
                Times.Once, "The 'TransferMoney' method of the repository is not called correctly.");

            Assert.That(_window.IsVisible && _window.IsActive, Is.True, "The window should not close itself when the transaction fails.");

            Assert.That(_errorMessageTextBlock.Visibility, Is.EqualTo(Visibility.Visible),
                "The 'Visibility' of the error message should be visible.");
            Assert.That(_errorMessageTextBlock.Text, Contains.Substring(failureResult.Message),
                "The error message should contain the message returned by the service.");
        }

        private void SelectTheToAccountInTheComboBox()
        {
            Assert.That(_toAccountsComboBox.Items.Count, Is.EqualTo(_allAccountsOfCustomer.Count),
                "The combobox should have an item for each account of the customer.");
            _toAccountsComboBox.SelectedIndex = 1; //select toAccount
        }

        private void InitializeWindow()
        {
            _fromAccount = new AccountBuilder().Build();
            _toAccount = new AccountBuilder().Build();
            _allAccountsOfCustomer = new List<Account> { _fromAccount, _toAccount };

            _window = new TransferWindow(_fromAccount, _allAccountsOfCustomer, _accountServiceMock.Object);
            _window.Show();

            _fromAccountTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("FromAccountTextBlock");
            _toAccountsComboBox = _window.FindVisualChildren<ComboBox>().First();
            _amountTextBox = _window.GetPrivateFieldValueByName<TextBox>("AmountTextBox");
            _errorMessageTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("ErrorMessageTextBlock");
            _transferButton = _window.GetPrivateFieldValueByName<Button>("TransferButton");
        }
    }
}
