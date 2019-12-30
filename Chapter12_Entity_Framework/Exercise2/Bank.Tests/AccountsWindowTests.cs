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
using System.Windows.Controls;
using System.Windows.Data;
using Bank.Business;
using Bank.Business.Interfaces;
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise02",
        @"Bank.Data\DomainClasses\Account.cs;
Bank.Data\DomainClasses\Customer.cs;
Bank.Data\BankContext.cs;
Bank.Data\AccountRepository.cs;
Bank.Data\CityRepository.cs;
Bank.Data\CustomerRepository.cs;
Bank.Business\AccountValidator.cs;
Bank.Business\CustomerValidator.cs;
Bank.UI\AccountsWindow.xaml;
Bank.UI\AccountsWindow.xaml.cs;
Bank.UI\CustomersWindow.xaml;
Bank.UI\CustomersWindow.xaml.cs;
Bank.UI\TransferWindow.xaml;
Bank.UI\TransferWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class AccountsWindowTests
    {
        private AccountsWindow _window;
        private DataGrid _datagrid;
        private Button _addAccountButton;
        private Button _saveAccountButton;
        private Button _transferButton;
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<IWindowDialogService> _windowDialogServiceMock;
        private Mock<IAccountValidator> _accountValidatorMock;
        private TextBlock _errorTextBlock
            ;

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountValidatorMock = new Mock<IAccountValidator>();
            _accountValidatorMock.Setup(validator => validator.IsValid(It.IsAny<Account>()))
                .Returns(ValidatorResult.Success);
            _windowDialogServiceMock = new Mock<IWindowDialogService>();
        }

        [TearDown]
        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("AccountsWindow - Should have configured the account datagrid columns correctly in XAML"), Order(1)]
        public void _01_ShouldHaveConfiguredTheAccountDataGridColumnsCorrectlyInXaml()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().WithAccounts(new List<Account>()).Build();
            InitializeWindow(customer);

            var dataGridTextColumns = _datagrid.Columns.OfType<DataGridTextColumn>().ToList();
            var textColumnBindings = dataGridTextColumns.Select(column => column.Binding).OfType<Binding>().ToList();
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "AccountNumber"),
                "Could not find a binding for the 'AccountNumber' property.");
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "Balance"),
                 "Could not find a binding for the 'Balance' property.");

            var balanceColumn = dataGridTextColumns.First(column => (column.Header as string)?.ToLower() == "balance");
            var balanceBinding = (Binding)balanceColumn.Binding;
            Assert.That(balanceBinding.Mode, Is.EqualTo(BindingMode.OneWay),
                "Set the binding mode of the balance column so that " +
                "updates in the UI are ignored, but updates on the source object are detected.");
        }

        [MonitoredTest("AccountsWindow - Should show the fullname of the customer in its title"), Order(2)]
        public void _02_ShouldShowTheFullNameOfTheCustomerInItsTitle()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().WithAccounts(new List<Account>()).Build();

            //Act
            InitializeWindow(customer);

            //Assert
            var fullName = $"{customer.FirstName} {customer.Name}".Trim();
            Assert.That(_window.Title, Contains.Substring(fullName),
                "Title does not contain the correct concatenation of 'FirstName', whitespace, 'LastName'.");
        }

        [MonitoredTest("AccountsWindow - Should use a list with the accounts of the customer as a the source for the datagrid"), Order(3)]
        public void _03_ShouldUseAListWithTheAccountsOfTheCustomerAsSourceForTheDataGrid()
        {
            //Arrange
            var accounts = new HashSet<Account>
            {
                new AccountBuilder().Build(),
                new AccountBuilder().Build(),
                new AccountBuilder().Build(),
            };
            var customer = new CustomerBuilder().WithId().WithAccounts(accounts).Build();

            //Act
            InitializeWindow(customer);

            //Assert
            Assert.That(_datagrid.ItemsSource, Is.InstanceOf<IList<Account>>(),
                "The ItemsSource of the DataGrid should be an object that implements IList<Account>. " +
                "Tip: copy the accounts of the customer into a new List. " +
                "This can be done with one line of code if you use the right constructor.");
            Assert.That(_datagrid.ItemsSource, Is.EquivalentTo(accounts),
                "The Account instances in the ItemsSource of the DataGrid should be the same instances that can be found in the Accounts of the Customer.");
        }

        [MonitoredTest("AccountsWindow - A click on AddAccountButton should add a new row"), Order(4)]
        public void _04_AddAccountButton_Click_ShouldAddANewRow()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().WithAccounts(new List<Account>()).Build();
            InitializeWindow(customer);

            //Act
            _addAccountButton.FireClickEvent();

            //Assert
            Assert.That(_datagrid.CanUserAddRows, Is.True,
                "The property 'CanUserAddRows' of the datagrid should be true. " +
                "This ensures that a new (empty) row is added to the datagrid.");
        }

        [MonitoredTest("AccountsWindow - A click on SaveAccountButton should update a selected existing account in the database"), Order(5)]
        public void _05_SaveAccountButton_Click_ShouldUpdateASelectedExistingAccountInTheDatabase()
        {
            //Arrange
            var existingAccount = new AccountBuilder().WithId().Build();
            AddAccountsToTheGridAndSelectTheFirst(new List<Account> { existingAccount });
            _errorTextBlock.Text = "Some error message";

            //Act
            _saveAccountButton.FireClickEvent();

            //Assert
            _accountRepositoryMock.Verify(repo => repo.Update(existingAccount), Times.Once,
                "The 'Update' method of the repository is not called correctly.");
            _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
            _accountValidatorMock.Verify(validator => validator.IsValid(existingAccount), Times.Once,
                "The validator is not used correctly to check if the customer is valid.");
            Assert.That(_errorTextBlock.Text, Is.Empty,
                "When saving, previous error messages should be cleared from the ErrorTextBlock.");
        }

        [MonitoredTest("AccountsWindow - A click on SaveAccountButton should add a selected new account in the database"), Order(6)]
        public void _06_SaveAccountButton_Click_ShouldAddASelectedNewAccountToTheDatabase()
        {
            //Arrange  
            var newAccount = new AccountBuilder().WithId(0).Build();
            AddNewAccountToTheGridAndSelectIt(newAccount);

            //Act
            _saveAccountButton.FireClickEvent();

            //Assert
            _accountRepositoryMock.Verify(repo => repo.Add(newAccount), Times.Once,
                "The 'Add' method of the repository is not called correctly.");
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never,
                "The 'Update' method of the repository should not have been called.");
            _accountValidatorMock.Verify(validator => validator.IsValid(newAccount), Times.Once,
                "The validator is not used correctly to check if the customer is valid.");
            Assert.That(_datagrid.CanUserAddRows, Is.False);
        }

        [MonitoredTest("AccountsWindow - A click on SaveAccountButton should show an error when no account is selected"), Order(7)]
        public void _07_SaveAccountButton_Click_ShouldShowAnErrorWhenNoAccountIsSelected()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().WithAccounts(new List<Account>()).Build();
            InitializeWindow(customer);
            _datagrid.SelectedIndex = -1;
            _errorTextBlock.Text = "";

            //Act
            Assert.That(() => _saveAccountButton.FireClickEvent(), Throws.Nothing,
                 "An exception occurs when nothing is selected.");

            //Assert
            _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never,
                "The 'Update' method of the repository should not have been called.");
            Assert.That(_errorTextBlock.Text, Is.Not.Null.And.Not.Empty, "No error message is shown in the ErrorTextBlock.");
        }

        [MonitoredTest("AccountsWindow - A click on SaveAccountButton should show an error when the selected account is invalid"), Order(8)]
        public void _08_SaveAccountButton_Click_ShouldShowAnErrorWhenTheSelectedAccountIsInvalid()
        {
            //Arrange
            var existingAccount = new AccountBuilder().WithId().Build();
            AddAccountsToTheGridAndSelectTheFirst(new List<Account> { existingAccount });
            _errorTextBlock.Text = "";

            var expectedErrorMessage = Guid.NewGuid().ToString();
            _accountValidatorMock.Setup(validator => validator.IsValid(It.IsAny<Account>()))
                .Returns(ValidatorResult.Fail(expectedErrorMessage));

            //Act
            _saveAccountButton.FireClickEvent();

            //Assert
            _accountValidatorMock.Verify(validator => validator.IsValid(existingAccount), Times.Once,
                "The validator is not used correctly to check if the account is valid.");
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never,
                "The 'Update' method of the repository should not have been called.");
            _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");

            Assert.That(_errorTextBlock.Text, Is.EqualTo(expectedErrorMessage),
                "The ErrorTextBlock should contain the error message in de failed ValidatorResult.");
        }

        [MonitoredTest("AccountsWindow - The handler for TransferButton should not directly create an instance of 'TransferWindow'"), Order(9)]
        public void _09_TheTransferButtonHandlerShouldNotDirectlyCreateAnInstanceOfTransferWindow()
        {
            //Arrange
            var sourceCode = Solution.Current.GetFileContent(@"Bank.UI\AccountsWindow.xaml.cs");
            sourceCode = CodeCleaner.StripComments(sourceCode);

            //Assert
            Assert.That(sourceCode, Does.Not.Contain("new TransferWindow("),
                () => "Code found where a new instance of 'TransferWindow' is created. " +
                      "Use the 'IWindowDialogService' instead to show the transfer window.");
        }

        [MonitoredTest("AccountsWindow - A click on TransferButton should show the TransferWindow"), Order(10)]
        public void _10_TransferButton_Click_ShouldShowTheTransferWindow()
        {
            //Arrange
            var fromAccount = new AccountBuilder().WithId().Build();
            var allAccountsOfCustomer = new List<Account> { fromAccount, new AccountBuilder().WithId().Build() };
            AddAccountsToTheGridAndSelectTheFirst(allAccountsOfCustomer);

            //Act
            _transferButton.FireClickEvent();

            //Assert
            _windowDialogServiceMock.Verify(
                service => service.ShowTransferDialog(fromAccount,
                    It.Is<IList<Account>>(accounts => accounts.All(account =>
                        allAccountsOfCustomer.Any(otherAccount =>
                            account.AccountNumber == otherAccount.AccountNumber)))), Times.Once,
                "A call to the 'ShowTranferDialog' method of the 'IWindowDialogService' should have been made correctly. " +
                "The first parameter should be the selected account in the datagrid. " +
                "The second parameter should be a list of all the accounts of the customer. " +
                "(Maybe the same list you retrieve in the constructor?)");
        }

        [MonitoredTest("AccountsWindow - A click on TransferButton should do nothing when no account is selected"), Order(11)]
        public void _11_TransferButton_ShouldDoNothingWhenNoAccountIsSelected()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().WithAccounts(new List<Account>()).Build();
            InitializeWindow(customer);
            _datagrid.SelectedIndex = -1;

            //Act
            Assert.That(() => _transferButton.FireClickEvent(), Throws.Nothing,
                () => "An exception occurs when nothing is selected.");

            //Assert
            _windowDialogServiceMock.Verify(
                service => service.ShowTransferDialog(It.IsAny<Account>(), It.IsAny<IList<Account>>()), Times.Never,
                "The 'ShowTransferDialog' method of the 'IWindowDialogService' should not have been called.");
        }

        private void InitializeWindow(Customer customer)
        {
            _window = new AccountsWindow(customer, _accountRepositoryMock.Object, _accountValidatorMock.Object, _windowDialogServiceMock.Object);
            _window.Show();

            _datagrid = _window.FindVisualChildren<DataGrid>().FirstOrDefault();
            _addAccountButton = _window.GetPrivateFieldValueByName<Button>("AddAccountButton");
            _saveAccountButton = _window.GetPrivateFieldValueByName<Button>("SaveAccountButton");
            _transferButton = _window.GetPrivateFieldValueByName<Button>("TransferButton");
            _errorTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("ErrorTextBlock");
        }

        private void AddAccountsToTheGridAndSelectTheFirst(IList<Account> allAccountsOfCustomer)
        {
            var customer = new CustomerBuilder().WithId().WithAccounts(allAccountsOfCustomer).Build();

            InitializeWindow(customer);

            _datagrid.SelectedIndex = 0; //select the account
        }

        private void AddNewAccountToTheGridAndSelectIt(Account newAccount)
        {
            var customer = new CustomerBuilder().WithId().WithAccounts(new List<Account>()).Build();

            InitializeWindow(customer);

            _datagrid.CanUserAddRows = true;
            var itemsSource = _datagrid.ItemsSource as IList<Account>;
            Assert.That(itemsSource, Is.Not.Null,
                "The ItemsSource of the datagrid should be a collection that is assignable to an IList<Account>.");

            var newItemsSource = new List<Account>(itemsSource);
            newItemsSource.Insert(0, newAccount);

            _datagrid.ItemsSource = newItemsSource;
            _datagrid.SelectedIndex = 0; //select the account
        }
    }
}
