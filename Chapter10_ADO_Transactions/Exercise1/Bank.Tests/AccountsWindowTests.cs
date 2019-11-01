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
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise01",
         @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
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

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountRepositoryMock.Setup(repo => repo.GetAllAccountsOfCustomer(It.IsAny<int>())).Returns(new List<Account>());

            _windowDialogServiceMock = new Mock<IWindowDialogService>();
        }

        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("AccountsWindow - Should have configured the account datagrid columns correctly in XAML"), Order(1)]
        public void _1_ShouldHaveConfiguredTheAccountDataGridColumnsCorrectlyInXaml()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().Build();
            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);

            var dataGridTextColumns = _datagrid.Columns.OfType<DataGridTextColumn>().ToList();
            var textColumnBindings = dataGridTextColumns.Select(column => column.Binding).OfType<Binding>().ToList();
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "AccountNumber"),
                () => "Could not find a binding for the 'AccountNumber' property.");
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "Balance"),
                () => "Could not find a binding for the 'Balance' property.");

            var balanceColumn = dataGridTextColumns.First(column => (column.Header as string)?.ToLower() == "balance");
            Assert.That(balanceColumn.IsReadOnly, Is.True,
                () =>
                    "The 'Balance' column should be readonly ('IsReadOnly') " +
                    "so that it is not possible to edit balances directly in the datagrid.");
            var balanceBinding = (Binding)balanceColumn.Binding;
            Assert.That(balanceBinding.Mode, Is.EqualTo(BindingMode.OneWay),
                () =>
                    "Set the binding mode of the balance column so that " +
                    "updates in the UI are ignored, but updates on the source object are detected.");
        }

        [MonitoredTest("AccountsWindow - Should load the accounts of the customer on construction"), Order(2)]
        public void _2_ShouldLoadTheAccountsOfTheCustomerOnConstruction()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().Build();
            var allAccounts = new List<Account>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountRepositoryMock.Setup(repo => repo.GetAllAccountsOfCustomer(It.IsAny<int>())).Returns(allAccounts);

            //Act
            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);

            //Assert
            _accountRepositoryMock.Verify(repo => repo.GetAllAccountsOfCustomer(customer.CustomerId), Times.Once,
                "The constructor should call the 'GetAllAccountsOfCustomer' method from the repository correctly.");
            Assert.That(_datagrid.ItemsSource, Is.SameAs(allAccounts),
                () => "The 'ItemsSource' of the datagrid should be the very same list returned by the repository.");

        }

        [MonitoredTest("AccountsWindow - Should show the fullname of the customer in its title"), Order(3)]
        public void _3_ShouldShowTheFullNameOfTheCustomerInItsTitle()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().Build();

            //Act
            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);

            //Assert
            var fullName = $"{customer.FirstName} {customer.Name}".Trim();
            Assert.That(_window.Title, Contains.Substring(fullName),
                () => "Title does not contain the correct concatenation of 'FirstName', whitespace, 'LastName'.");
        }

        [MonitoredTest("AccountsWindow - A click on AddAccountButton should add a new row"), Order(4)]
        public void _4_AddAccountButton_Click_ShouldAddANewRow()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().Build();
            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);

            //Act
            _addAccountButton.FireClickEvent();

            //Assert
            Assert.That(_datagrid.CanUserAddRows, Is.True,
                () =>
                    "The property 'CanUserAddRows' of the datagrid should be true. " +
                    "This ensures that a new (empty) row is added to the datagrid.");
        }

        [MonitoredTest("AccountsWindow - A click on SaveAccountButton should update a selected existing account in the database"), Order(5)]
        public void _5_SaveAccountButton_Click_ShouldUpdateASelectedExistingAccountInTheDatabase()
        {
            //Arrange
            var existingAccount = new AccountBuilder().WithId().Build();
            AddAccountsToTheGridAndSelectTheFirst(new List<Account> { existingAccount });

            //Act
            _saveAccountButton.FireClickEvent();

            //Assert
            _accountRepositoryMock.Verify(repo => repo.Update(existingAccount), Times.Once,
                "The 'Update' method of the repository is not called correctly.");
            _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
        }

        [MonitoredTest("AccountsWindow - A click on SaveAccountButton should add a selected new account in the database"), Order(6)]
        public void _6_SaveAccountButton_Click_ShouldAddASelectedNewAccountToTheDatabase()
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

            Assert.That(_datagrid.CanUserAddRows, Is.False);
        }

        [MonitoredTest("AccountsWindow - A click on SaveAccountButton should do nothing when no account is selected"), Order(7)]
        public void _7_SaveAccountButton_Click_ShouldDoNothingWhenNoAccountIsSelected()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().Build();
            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);
            _datagrid.SelectedIndex = -1;

            //Act
            Assert.That(() => _saveAccountButton.FireClickEvent(), Throws.Nothing,
                () => "An exception occurs when nothing is selected.");

            //Assert
            _accountRepositoryMock.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never,
                "The 'Update' method of the repository should not have been called.");
        }

        [MonitoredTest("CustomersWindow - The handler for TransferButton should not directly create an instance of 'TransferWindow'"), Order(8)]
        public void _8_TheTransferButtonHandlerShouldNotDirectlyCreateAnInstanceOfTransferWindow()
        {
            //Arrange
            var sourceCode = Solution.Current.GetFileContent(@"Bank.UI\AccountsWindow.xaml.cs");
            sourceCode = CodeCleaner.StripComments(sourceCode);

            //Assert
            Assert.That(sourceCode, Does.Not.Contain("new TransferWindow("),
                () => "Code found where a new instance of 'TransferWindow' is created. " +
                      "Use the 'IWindowDialogService' instead to show the transfer window.");
        }

        [MonitoredTest("AccountsWindow - A click on TransferButton should show the TransferWindow"), Order(9)]
        public void _9_TransferButton_Click_ShouldShowTheTransferWindow()
        {
            //Arrange
            var fromAccount = new AccountBuilder().WithId().Build();
            var allAccountsOfCustomer = new List<Account> { fromAccount, new AccountBuilder().WithId().Build() };
            _accountRepositoryMock.Setup(repo => repo.GetAllAccountsOfCustomer(It.IsAny<int>()))
                .Returns(allAccountsOfCustomer);
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

        [MonitoredTest("AccountsWindow - A click on TransferButton should do nothing when no account is selected"), Order(10)]
        public void _10_TransferButton_ShouldDoNothingWhenNoAccountIsSelected()
        {
            //Arrange
            var customer = new CustomerBuilder().WithId().Build();
            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);
            _datagrid.SelectedIndex = -1;

            //Act
            Assert.That(() => _transferButton.FireClickEvent(), Throws.Nothing,
                () => "An exception occurs when nothing is selected.");

            //Assert
            _windowDialogServiceMock.Verify(
                service => service.ShowTransferDialog(It.IsAny<Account>(), It.IsAny<IList<Account>>()), Times.Never,
                "The 'ShowTransferDialog' method of the 'IWindowDialogService' should not have been called.");
        }

        private void InitializeWindow(Customer customer, IAccountRepository accountRepository, IWindowDialogService windowDialogService)
        {
            _window = new AccountsWindow(customer, accountRepository, windowDialogService);
            _window.Show();

            _datagrid = _window.FindVisualChildren<DataGrid>().FirstOrDefault();
            _addAccountButton = _window.GetPrivateFieldValueByName<Button>("AddAccountButton");
            _saveAccountButton = _window.GetPrivateFieldValueByName<Button>("SaveAccountButton");
            _transferButton = _window.GetPrivateFieldValueByName<Button>("TransferButton");
        }

        private void AddAccountsToTheGridAndSelectTheFirst(IList<Account> allAccountsOfCustomer)
        {
            var customer = new CustomerBuilder().WithId().Build();
            _accountRepositoryMock.Setup(repo => repo.GetAllAccountsOfCustomer(It.IsAny<int>()))
                .Returns(allAccountsOfCustomer);

            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);

            _datagrid.SelectedIndex = 0; //select the account
        }

        private void AddNewAccountToTheGridAndSelectIt(Account newAccount)
        {
            var customer = new CustomerBuilder().WithId().Build();
            var accountsItemSource = new List<Account>();
            _accountRepositoryMock.Setup(repo => repo.GetAllAccountsOfCustomer(It.IsAny<int>()))
                .Returns(accountsItemSource);

            InitializeWindow(customer, _accountRepositoryMock.Object, _windowDialogServiceMock.Object);

            _datagrid.CanUserAddRows = true;
            accountsItemSource.Insert(0, newAccount);

            _datagrid.SelectedIndex = 0; //select the account
        }
    }
}
