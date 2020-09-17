using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using Bank.Business;
using Bank.Business.Interfaces;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;
using Bank.UI;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Moq;
using NUnit.Framework;

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
    public class CustomersWindowTests
    {
        private CustomersWindow _window;
        private DataGrid _datagrid;
        private Button _addCustomerButton;
        private Button _saveCustomerButton;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<ICityRepository> _cityRepositoryMock;
        private Mock<IWindowDialogService> _windowDialogServiceMock;
        private Mock<ICustomerValidator> _customerValidatorMock;
        private Button _showAccountsButton;
        private TextBlock _errorTextBlock;


        [SetUp]
        public void Setup()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerRepositoryMock.Setup(repo => repo.GetAllWithAccounts()).Returns(new List<Customer>());
            _cityRepositoryMock = new Mock<ICityRepository>();
            _cityRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<City>());
            _customerValidatorMock = new Mock<ICustomerValidator>();
            _customerValidatorMock.Setup(validator => validator.IsValid(It.IsAny<Customer>())).Returns(ValidatorResult.Success);
            _windowDialogServiceMock = new Mock<IWindowDialogService>();
        }

        [TearDown]
        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("CustomersWindow - Should have configured the customer datagrid columns correctly in XAML"), Order(1)]
        public void _01_ShouldHaveConfiguredTheCustomerDataGridColumnsCorrectlyInXaml()
        {
            InitializeWindow();

            var textColumnBindings = _datagrid.Columns.OfType<DataGridTextColumn>().Select(column => column.Binding).OfType<Binding>().ToList();
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "Name"),
                "Could not find a binding for the 'Name' property.");
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "FirstName"),
                "Could not find a binding for the 'FirstName' property.");
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "Address"),
                 "Could not find a binding for the 'Address' property.");
            Assert.That(textColumnBindings, Has.One.Matches((Binding binding) => binding.Path.Path == "CellPhone"),
                 "Could not find a binding for the 'CellPhone' property.");

            var comboBoxColumn = _datagrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault();
            Assert.That(comboBoxColumn, Is.Not.Null, () => "Could not find a DataGridComboBoxColumn in the datagrid.");

            var comboBoxColumnBinding = comboBoxColumn.SelectedValueBinding as Binding;
            var useSelectedValueBindingMessage =
                "The selected value of the combobox should be bound to the 'ZipCode' property of the 'Customer' ('SelectedValueBinding').";
            Assert.That(comboBoxColumnBinding, Is.Not.Null, useSelectedValueBindingMessage);
            Assert.That(comboBoxColumnBinding.Path.Path, Is.EqualTo("ZipCode"), useSelectedValueBindingMessage);
            Assert.That(comboBoxColumn.SelectedValuePath, Is.EqualTo("ZipCode"),
                "The 'ZipCode' property of 'City' should be used as value for each item in the combobox ('SelectedValuePath')");
            Assert.That(comboBoxColumn.DisplayMemberPath, Is.EqualTo("Name"),
                "The 'Name' property of 'City' should be used as display text for each item in the combobox ('DisplayMemberPath')");
        }

        [MonitoredTest("CustomersWindow - Should load the customers on construction"), Order(2)]
        public void _02_ShouldLoadTheCustomersOnConstruction()
        {
            //Arrange
            var allCustomers = new List<Customer>();
            _customerRepositoryMock.Setup(repo => repo.GetAllWithAccounts()).Returns(allCustomers);

            //Act
            InitializeWindow();

            //Assert
            _customerRepositoryMock.Verify(repo => repo.GetAllWithAccounts(), Times.Once,
                "The constructor should call the 'GetAll' method from the customer repository correctly.");
            Assert.That(_datagrid.ItemsSource, Is.SameAs(allCustomers),
                "The 'ItemsSource' of the datagrid should be the very same list returned by the repository.");
        }

        [MonitoredTest("CustomersWindow - Should load the cities on construction"), Order(3)]
        public void _03_ShouldLoadTheCitiesOnConstruction()
        {
            //Arrange
            var allCities = new List<City>();
            _cityRepositoryMock.Setup(repo => repo.GetAll()).Returns(allCities);

            //Act
            InitializeWindow();

            //Assert
            var comboBoxColumn = _window.GetPrivateFieldValue<DataGridComboBoxColumn>();
            _cityRepositoryMock.Verify(repo => repo.GetAll(), Times.Once,
                "The constructor should call the 'GetAll' method from the city repository correctly.");
            Assert.That(comboBoxColumn.ItemsSource, Is.EqualTo(allCities),
                "The 'ItemsSource' of the combobox column of the datagris should be the list returned by the repository.");
        }

        [MonitoredTest("CustomersWindow - A click on AddCustomerButton should add a new row"), Order(4)]
        public void _04_AddCustomerButton_Click_ShouldAddANewRow()
        {
            //Arrange
            InitializeWindow();

            //Act
            _addCustomerButton.FireClickEvent();

            //Assert
            Assert.That(_datagrid.CanUserAddRows, Is.True,
                () =>
                    "The property 'CanUserAddRows' of the datagrid should be true. " +
                    "This ensures that a new (empty) row is added to the datagrid.");
        }

        [MonitoredTest("CustomersWindow - A click on SaveCustomeButton should update a selected existing customer in the database"), Order(5)]
        public void _05_SaveCustomerButton_Click_ShouldUpdateASelectedExistingCustomerInTheDatabase()
        {
            //Arrange
            var existingCustomer = new CustomerBuilder().WithId().Build();
            AddCustomerToTheGridAndSelectIt(existingCustomer);
            _errorTextBlock.Text = "Some error message";
            _customerRepositoryMock.Invocations.Clear();

            //Act
            _saveCustomerButton.FireClickEvent();

            //Assert
            _customerRepositoryMock.Verify(repo => repo.Update(existingCustomer), Times.Once,
                "The 'Update' method of the repository is not called correctly.");
            _customerRepositoryMock.Verify(repo => repo.Add(It.IsAny<Customer>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
            _customerRepositoryMock.Verify(repo => repo.GetAllWithAccounts(), Times.Never,
                "There is no need to call the 'GetAllWithAccounts' method of the CustomerRepository.");
            _customerValidatorMock.Verify(validator => validator.IsValid(existingCustomer), Times.Once,
                "The validator is not used correctly to check if the customer is valid.");
            Assert.That(_errorTextBlock.Text, Is.Empty,
                "When saving, previous error messages should be cleared from the ErrorTextBlock.");
        }

        [MonitoredTest("CustomersWindow - A click on SaveCustomeButton should add a selected new customer to the database"), Order(6)]
        public void _06_SaveCustomerButton_Click_ShouldAddASelectedNewCustomerToTheDatabase()
        {
            //Arrange
            var newCustomer = new CustomerBuilder().WithId(0).Build();
            AddCustomerToTheGridAndSelectIt(newCustomer);
            _customerRepositoryMock.Invocations.Clear();

            //Act
            _saveCustomerButton.FireClickEvent();

            //Assert
            _customerRepositoryMock.Verify(repo => repo.Add(newCustomer), Times.Once,
                "The 'Add' method of the repository is not called correctly.");
            _customerRepositoryMock.Verify(repo => repo.Update(It.IsAny<Customer>()), Times.Never,
                "The 'Update' method of the repository should not have been called.");
            _customerRepositoryMock.Verify(repo => repo.GetAllWithAccounts(), Times.Never,
                "There is no need to call the 'GetAllWithAccounts' method of the CustomerRepository.");
            _customerValidatorMock.Verify(validator => validator.IsValid(newCustomer), Times.Once,
                "The validator is not used correctly to check if the customer is valid.");
            Assert.That(_datagrid.CanUserAddRows, Is.False, () => "After adding a new customer the property 'CanUserAddRows' of the datagrid should be false " +
                                                                  "so that the new row is showed as a normal row.");
        }

        [MonitoredTest("CustomersWindow - A click on SaveCustomeButton should show an error when no customer is selected"), Order(7)]
        public void _07_SaveCustomerButton_Click_ShouldShowAnErrorWhenNoCustomerIsSelected()
        {
            //Arrange
            InitializeWindow();
            _datagrid.SelectedIndex = -1;
            _errorTextBlock.Text = "";

            //Act
            Assert.That(() => _saveCustomerButton.FireClickEvent(), Throws.Nothing, "An exception occurs when nothing is selected.");

            //Assert
            _customerRepositoryMock.Verify(repo => repo.Add(It.IsAny<Customer>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");
            _customerRepositoryMock.Verify(repo => repo.Update(It.IsAny<Customer>()), Times.Never,
                "The 'Update' method of the repository should not have been called.");
            Assert.That(_errorTextBlock.Text, Is.Not.Null.And.Not.Empty, "No error message is shown in the ErrorTextBlock.");
        }

        [MonitoredTest("CustomersWindow - A click on SaveCustomeButton should show an error when the selected customer is invalid"), Order(8)]
        public void _08_SaveCustomerButton_Click_ShouldShowAnErrorWhenTheSelectedCustomerIsInvalid()
        {
            //Arrange
            var existingCustomer = new CustomerBuilder().WithId().Build();
            AddCustomerToTheGridAndSelectIt(existingCustomer);
            _errorTextBlock.Text = "";

            var expectedErrorMessage = Guid.NewGuid().ToString();
            _customerValidatorMock.Setup(validator => validator.IsValid(It.IsAny<Customer>()))
                .Returns(ValidatorResult.Fail(expectedErrorMessage));

            //Act
            _saveCustomerButton.FireClickEvent();

            //Assert
            _customerValidatorMock.Verify(validator => validator.IsValid(existingCustomer), Times.Once,
                "The validator is not used correctly to check if the customer is valid.");
            _customerRepositoryMock.Verify(repo => repo.Update(It.IsAny<Customer>()), Times.Never,
                "The 'Update' method of the repository should not have been called.");
            _customerRepositoryMock.Verify(repo => repo.Add(It.IsAny<Customer>()), Times.Never,
                "The 'Add' method of the repository should not have been called.");

            Assert.That(_errorTextBlock.Text, Is.EqualTo(expectedErrorMessage),
                "The ErrorTextBlock should contain the error message in de failed ValidatorResult.");
        }

        [MonitoredTest("CustomersWindow - The handler for ShowAccountsButton should not directly create an instance of 'AccountsWindow'"), Order(9)]
        public void _09_TheShowAccountsButtonHandlerShouldNotDirectlyCreateAnInstanceOfAccountWindow()
        {
            //Arrange
            var sourceCode = Solution.Current.GetFileContent(@"Bank.UI\CustomersWindow.xaml.cs");
            sourceCode = CodeCleaner.StripComments(sourceCode);

            //Assert
            Assert.That(sourceCode, Does.Not.Contain("new AccountsWindow("),
                () => "Code found where a new instance of 'AccountsWindow' is created. " +
                      "Use the 'IWindowDialogService' instead to show the accounts window.");
        }

        [MonitoredTest("CustomersWindow - A click on ShowAccountsButton should show the AccountsWindow for the selected customer"), Order(10)]
        public void _10_ShowAccountsButton_Click_ShouldShowTheAccountsWindowForTheSelectedCustomer()
        {
            //Arrange
            var existingCustomer = new CustomerBuilder().WithId().Build();
            AddCustomerToTheGridAndSelectIt(existingCustomer);

            //Act
            _showAccountsButton.FireClickEvent();

            //Assert
            _windowDialogServiceMock.Verify(service => service.ShowAccountDialogForCustomer(existingCustomer), Times.Once,
                "A call to the 'ShowAccountDialogForCustomer' method of the 'IWindowDialogService' should have been made correctly. " +
                "The parameter should be the selected customer in the datagrid. ");
        }

        [MonitoredTest("CustomersWindow - A click on ShowAccountsButton should do nothing when no customer is selected"), Order(11)]
        public void _11_ShowAccountsButton_ShouldDoNothingWhenNoCustomerIsSelected()
        {
            //Arrange
            InitializeWindow();
            _datagrid.SelectedIndex = -1;

            //Act
            Assert.That(() => _showAccountsButton.FireClickEvent(), Throws.Nothing,
                () => "An exception occurs when nothing is selected.");

            //Assert
            _windowDialogServiceMock.Verify(service => service.ShowAccountDialogForCustomer(It.IsAny<Customer>()),
                Times.Never,
                "The 'ShowAccountDialogForCustomer' method of the 'IWindowDialogService' should not have been called.");
        }

        private void InitializeWindow()
        {
            _window = new CustomersWindow(_customerRepositoryMock.Object, _customerValidatorMock.Object, _cityRepositoryMock.Object, _windowDialogServiceMock.Object);
            _window.Show();

            _datagrid = _window.FindVisualChildren<DataGrid>().FirstOrDefault();
            _addCustomerButton = _window.GetPrivateFieldValueByName<Button>("AddCustomerButton");
            _saveCustomerButton = _window.GetPrivateFieldValueByName<Button>("SaveCustomerButton");
            _showAccountsButton = _window.GetPrivateFieldValueByName<Button>("ShowAccountsButton");
            _errorTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("ErrorTextBlock");
        }

        private void AddCustomerToTheGridAndSelectIt(Customer customer)
        {
            var allExistingCustomers = new List<Customer>();
            if (customer.Id > 0)
            {
                allExistingCustomers.Add(customer);
            }
            _customerRepositoryMock.Setup(repo => repo.GetAllWithAccounts()).Returns(allExistingCustomers);
            InitializeWindow();

            if (customer.Id == 0)
            {
                _datagrid.CanUserAddRows = true;
                _datagrid.ItemsSource = new List<Customer> { customer };
            }

            _datagrid.SelectedIndex = 0; //select the customer
        }
    }
}
