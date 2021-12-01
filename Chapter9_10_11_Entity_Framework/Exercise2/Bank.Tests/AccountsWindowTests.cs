using System;
using Bank.UI;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared.TestTools;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using Bank.AppLogic.Contracts;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class AccountsWindowTests
    {
        private static readonly Random Random = new Random();
        private AccountsWindow _window;
        private Button _addAccountButton;
        private Mock<IWindowDialogService> _windowDialogServiceMock;
        private Mock<IAccountService> _accountServiceMock;
        private TextBlock _errorTextBlock;
        private ListView _listView;
        private GroupBox _newAccountGroupBox;
        private ComboBox _typeComboBox;
        private TextBox _accountNumberTextBox;

        private string _windowClassContent;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _windowClassContent = Solution.Current.GetFileContent(@"Bank.UI\AccountsWindow.xaml.cs");
        }

        [SetUp]
        public void Setup()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _windowDialogServiceMock = new Mock<IWindowDialogService>();
        }

        [TearDown]
        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("AccountsWindow - Should show the fullname of the customer in its title"), Order(1)]
        public void _01_ShouldShowTheFullNameOfTheCustomerInItsTitle()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().Build();

            //Act
            InitializeWindow(customer);

            //Assert
            var fullName = $"{customer.FirstName} {customer.Name}".Trim();
            Assert.That(_window.Title, Contains.Substring(fullName),
                "Title does not contain the correct concatenation of 'FirstName', whitespace, 'LastName'.");
        }

        [MonitoredTest("AccountsWindow - Constructor - Should use the accounts of the customer as the source for the ListView"), Order(2)]
        public void _02_Constructor_ShouldUseTheAccountsOfTheCustomerAsTheSourceForTheListView()
        {
            //Arrange
            Customer customer = new CustomerBuilder().WithId().WithAccounts().Build();

            //Act
            InitializeWindow(customer);

            //Assert
            ICollection<Account> accounts = customer.TryGetAccounts();
            Assert.That(_listView.ItemsSource, Is.SameAs(accounts),
                "The ItemsSource of the ListView should be the very same list of Accounts of the customer.");
        }

        [MonitoredTest("AccountsWindow - AddAccountButton click - Account service fails - Should show error"), Order(3)]
        public void _03_AddAccountButton_Click_AccountServiceFails_ShouldShowError()
        {
            //Arrange
            Customer customer = new CustomerBuilder().WithId().WithAccounts().Build();
            InitializeWindow(customer);

            string accountNumber = Guid.NewGuid().ToString();
            _accountNumberTextBox.Text = accountNumber;

            AccountType type = Random.NextAccountType();
            _typeComboBox.SelectedValue = type;

            Result failureResult = Result.Fail(Guid.NewGuid().ToString());
            _accountServiceMock
                .Setup(service =>
                    service.AddNewAccountForCustomer(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<AccountType>()))
                .Returns(failureResult);

            //Act
            Assert.That(string.IsNullOrEmpty(_errorTextBlock.Text), Is.True,
                "The error TextBlock should be empty before the add button is clicked.");

            _addAccountButton.FireClickEvent();

            //Assert
            _accountServiceMock.Verify(service => service.AddNewAccountForCustomer(customer, accountNumber, type), Times.Once,
                "The AddNewAccountForCustomer method of the account service is not called correctly.");

            Assert.That(string.IsNullOrEmpty(_errorTextBlock.Text), Is.False,
                "The error TextBlock should not be empty after the add button is clicked and the service fails.");

            Assert.That(_errorTextBlock.Text, Does.Contain(failureResult.Message),
                "The message in the Result returned by the account service should be in the error TextBlock.");
        }

        [MonitoredTest("AccountsWindow - AddAccountButton click - Account service succeeds - ShouldResetUI"), Order(4)]
        public void _04_AddAccountButton_Click_AccountServiceSucceeds_ShouldResetUI()
        {
            //Arrange
            Customer customer = new CustomerBuilder().WithId().WithAccounts().Build();
            InitializeWindow(customer);

            _errorTextBlock.Text = "Some previous error message";

            string accountNumber = Guid.NewGuid().ToString();
            _accountNumberTextBox.Text = accountNumber;

            AccountType type = Random.NextAccountType();
            _typeComboBox.SelectedValue = type;

            ICollection<Account> accounts = customer.TryGetAccounts();

            Result successResult = Result.Success();
            _accountServiceMock
                .Setup(service =>
                    service.AddNewAccountForCustomer(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<AccountType>()))
                .Returns(successResult).Callback(() =>
                {
                    accounts.Add(new AccountBuilder().WithCustomerId(customer.Id).Build());
                });

            //Act
            _addAccountButton.FireClickEvent();

            //Assert
            Assert.That(string.IsNullOrEmpty(_errorTextBlock.Text), Is.True,
                "The error TextBlock should be empty after the add button is clicked. Any previous error message should be cleared.");

            _accountServiceMock.Verify(service => service.AddNewAccountForCustomer(customer, accountNumber, type), Times.Once,
                "The AddNewAccountForCustomer method of the account service is not called correctly.");

            Assert.That(string.IsNullOrEmpty(_accountNumberTextBox.Text), Is.True,
                "The account TextBox should be cleared after the add succeeds.");

            ItemCollection items = _listView.Items;
            Assert.That(items.Count, Is.EqualTo(accounts.Count),
                "The 'Items' collection of the ListView should have one account more after the account is added. " +
                "Tip: try to call the 'Refresh' method on the 'Items' collection to tell WPF that the number of accounts of the customer changed.");
        }

        [MonitoredTest("AccountsWindow - TransferButton click - Should use window dialog service"), Order(5)]
        public void _05_TransferButton_Click_ShouldUseWindowDialogService()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_windowClassContent);
            var root = syntaxTree.GetRoot();
            MethodDeclarationSyntax transferButtonClickMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("TransferButton_Click"));

            Assert.That(transferButtonClickMethod, Is.Not.Null, "Cannot find a method 'TransferButton_Click' in AccountsWindow.xaml.cs");

            var bodyBuilder = new StringBuilder(); //no pun intended :)
            foreach (var statement in transferButtonClickMethod.Body.Statements)
            {
                bodyBuilder.AppendLine(statement.ToString());
            }
            string body = bodyBuilder.ToString();

            Assert.That(body, Contains.Substring(".ShowTransferDialog(selectedAccount,"),
                "The injected 'windowDialogService' should be used to show the accounts window.");

            Assert.That(body, Contains.Substring(".Accounts);"),
                "The injected 'windowDialogService' should be used to show the accounts window. " +
                "Make sure the 'Accounts' of the injected customer are passed in.");
        }

        private void InitializeWindow(Customer customer)
        {
            _window = new AccountsWindow(customer, _accountServiceMock.Object, _windowDialogServiceMock.Object);
            _window.Show();

            _listView = _window.FindVisualChildren<ListView>().FirstOrDefault();
            Assert.That(_listView, Is.Not.Null, "There should be a ListView defined in the XAML.");
            _newAccountGroupBox = _window.FindVisualChildren<GroupBox>().FirstOrDefault();
            Assert.That(_newAccountGroupBox, Is.Not.Null, "There should be a GroupBox defined in the XAML.");

            //AccountNumberTextBox
            _accountNumberTextBox = _window.GetPrivateFieldValueByName<TextBox>("AccountNumberTextBox");
            Assert.That(_accountNumberTextBox, Is.Not.Null, "There should be a TextBox with the name AccountNumberTextBox.");

            _typeComboBox = _window.FindVisualChildren<ComboBox>().FirstOrDefault();
            Assert.That(_typeComboBox, Is.Not.Null, "There should be a ComboBox defined in the XAML.");

            _addAccountButton = _window.GetPrivateFieldValueByName<Button>("AddAccountButton");
            Assert.That(_addAccountButton, Is.Not.Null, "There should be a Button with the name AddAccountButton.");
            _errorTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("ErrorTextBlock");
            Assert.That(_errorTextBlock, Is.Not.Null, "There should be a TextBlock with the name ErrorTextBlock.");
        }
    }
}
