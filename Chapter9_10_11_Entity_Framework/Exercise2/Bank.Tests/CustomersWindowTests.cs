using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;
using Bank.UI;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;")]
    [Apartment(ApartmentState.STA)]
    public class CustomersWindowTests
    {
        private static readonly Random Random = new Random();

        private CustomersWindow _window;
        private Button _addCustomerButton;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<ICityRepository> _cityRepositoryMock;
        private Mock<IWindowDialogService> _windowDialogServiceMock;
        private TextBlock _errorTextBlock;
        private ListView _listView;
        private List<City> _allCities;
        private ComboBox _cityComboBox;
        private GroupBox _newCustomerGroupBox;

        private string _windowClassContent;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _windowClassContent = Solution.Current.GetFileContent(@"Bank.UI\CustomersWindow.xaml.cs");
        }

        [SetUp]
        public void Setup()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerRepositoryMock.Setup(repo => repo.GetAllWithAccounts()).Returns(new List<Customer>());

            _allCities = new List<City>
            {
                new CityBuilder().Build(),
                new CityBuilder().Build(),
                new CityBuilder().Build()
            };
            _cityRepositoryMock = new Mock<ICityRepository>();
            _cityRepositoryMock.Setup(repo => repo.GetAllOrderedByZipCode()).Returns(_allCities);

            _windowDialogServiceMock = new Mock<IWindowDialogService>();
        }

        [TearDown]
        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("CustomersWindow - Constructor - Should load the customers"), Order(1)]
        public void _01_Constructor_ShouldLoadTheCustomers()
        {
            //Arrange
            var allCustomers = new List<Customer>();
            for (int i = 0; i < Random.Next(5, 11); i++)
            {
                allCustomers.Add(new CustomerBuilder().Build());
            }
            _customerRepositoryMock.Setup(repo => repo.GetAllWithAccounts()).Returns(allCustomers);

            //Act
            InitializeWindow();

            //Assert
            _customerRepositoryMock.Verify(repo => repo.GetAllWithAccounts(), Times.Once,
                "The constructor should call the 'GetAllWithAccounts' method from the customer repository correctly.");

            var observableCollection = _listView.ItemsSource as ObservableCollection<Customer>;
            Assert.That(observableCollection, Is.Not.Null,
                "The ItemsSource of the ListView should be an observable collection of customers. " +
                "This way the ListView will show an added customer.");

            Assert.That(observableCollection.All(customer => allCustomers.Any(c => c.Id == customer.Id)), Is.True,
                "The customers in the ItemsSource of the ListView should be the very same customers returned by the repository.");
        }

        [MonitoredTest("CustomersWindow - Constructor - Should load the cities"), Order(2)]
        public void _02_Constructor_ShouldLoadTheCities()
        {
            //Act
            InitializeWindow();

            //Assert
            _cityRepositoryMock.Verify(repo => repo.GetAllOrderedByZipCode(), Times.Once,
                "The constructor should call the 'GetAllOrderedByZipCode' method from the city repository.");
            Assert.That(_cityComboBox.ItemsSource, Is.SameAs(_allCities),
                "The ItemsSource of the ComboBox that holds the list of cities should be the exact same list of cities returned by the repository.");
        }

        [MonitoredTest("CustomersWindow - Constructor - Should set the DataContext of the NewCustomerGroupBox"), Order(3)]
        public void _03_Constructor_ShouldSetTheDataContextOfTheNewCustomerGroupBox()
        {
            //Act
            InitializeWindow();

            //Assert
            Customer dataContext = TryGetNewCustomerGroupBoxDataContext();

            Assert.That(dataContext.ZipCode, Is.EqualTo(_allCities.First().ZipCode),
                "The zip code of the GroupBox DataContext instance should be set to the zip code of the first city returned by the city repository.");
        }

        [MonitoredTest("CustomersWindow - AddCustomerButton click - Invalid customer - Should show error"), Order(4)]
        public void _04_AddCustomerButton_Click_InValidCustomer_ShouldShowError()
        {
            //Arrange
            InitializeWindow();

            Customer newCustomer = TryGetNewCustomerGroupBoxDataContext();
            newCustomer.Name = string.Empty; //Make sure the customer is invalid
            Result validationResult = newCustomer.Validate(_allCities);
            Assert.That(validationResult.IsSuccess, Is.False,
                "Customer validation is not yet implemented correctly. " +
                "Make sure to make the tests on 'Customer' green first.");
            string validationMessage = validationResult.Message;

            //Act
            Assert.That(string.IsNullOrEmpty(_errorTextBlock.Text), Is.True,
                "The error TextBlock should be empty before the add button is clicked.");

            _addCustomerButton.FireClickEvent();

            //Assert
            Assert.That(string.IsNullOrEmpty(_errorTextBlock.Text), Is.False,
                "The error TextBlock should not be empty after the add button is clicked and the Customer DataContext is invalid.");

            Assert.That(_errorTextBlock.Text, Does.Contain(validationMessage),
                "The message in the Result of the validation of the Customer should be in the error TextBlock.");

            _customerRepositoryMock.Verify(repo => repo.Add(It.IsAny<Customer>()), Times.Never,
                "The Add method of the customer repository should not be called when the customer is invalid.");
        }

        [MonitoredTest("CustomersWindow - AddCustomerButton click - Valid customer - Should add customer"), Order(5)]
        public void _05_AddCustomerButton_Click_ValidCustomer_ShouldAddCustomer()
        {
            //Arrange
            InitializeWindow();

            Customer newCustomer = TryGetNewCustomerGroupBoxDataContext();

            //make sure the customer is valid
            newCustomer.Name = Guid.NewGuid().ToString();
            newCustomer.FirstName = Guid.NewGuid().ToString();
            newCustomer.ZipCode = _allCities.Last().ZipCode;
            newCustomer.Address = Guid.NewGuid().ToString();

            Result validationResult = newCustomer.Validate(_allCities);
            Assert.That(validationResult.IsSuccess, Is.True,
                "Customer validation is not yet implemented correctly. " +
                "Make sure to make the tests on 'Customer' green first.");

            _errorTextBlock.Text = Guid.NewGuid().ToString(); //set a 'previous error'

            //Act
            _addCustomerButton.FireClickEvent();

            //Assert
            Assert.That(string.IsNullOrEmpty(_errorTextBlock.Text), Is.True,
                "The error TextBlock should be empty after the add button is clicked. " +
                "Make sure to clear a 'previous error message' before trying to add the customer.");

            _customerRepositoryMock.Verify(repo => repo.Add(It.IsAny<Customer>()), Times.Once,
                "The Add method of the customer repository should be called.");

            _customerRepositoryMock.Verify(repo => repo.Add(newCustomer), Times.Once,
                "The Add method of the customer repository is not called correctly. " +
                "Make sure the customer that is in the GroupBox DataContext is passed as parameter.");

            Customer nextNewCustomer = TryGetNewCustomerGroupBoxDataContext();
            Assert.That(nextNewCustomer, Is.Not.SameAs(newCustomer), "After adding the Customer, the DataContext of the GroupBox should point to a new instance of Customer.");

            Assert.That(nextNewCustomer.ZipCode, Is.EqualTo(_allCities.First().ZipCode),
                "The zip code of the next new Customer linked to the GroupBox DataContext instance should be set to the zip code of the first city returned by the city repository.");
        }

        [MonitoredTest("CustomersWindow - ShowAccountsButton click - Should use window dialog service"), Order(6)]
        public void _06_ShowAccountsButton_Click_ShouldUseWindowDialogService()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_windowClassContent);
            var root = syntaxTree.GetRoot();
            MethodDeclarationSyntax showButtonClickMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("ShowAccountsButton_Click"));

            Assert.That(showButtonClickMethod, Is.Not.Null, "Cannot find a method 'ShowAccountsButton_Click' in CustomersWindow.xaml.cs");

            var bodyBuilder = new StringBuilder(); //no pun intended :)
            foreach (var statement in showButtonClickMethod.Body.Statements)
            {
                bodyBuilder.AppendLine(statement.ToString());
            }
            string body = bodyBuilder.ToString();

            Assert.That(body, Contains.Substring(".ShowAccountDialogForCustomer(selectedCustomer);"),
                "The injected 'windowDialogService' should be used to show the accounts window.");
        }

        private void InitializeWindow()
        {
            _window = new CustomersWindow(_customerRepositoryMock.Object, _cityRepositoryMock.Object, _windowDialogServiceMock.Object);
            _window.Show();

            _listView = _window.FindVisualChildren<ListView>().FirstOrDefault();
            Assert.That(_listView, Is.Not.Null, "There should be a ListView defined in the XAML.");
            _cityComboBox = _window.FindVisualChildren<ComboBox>().FirstOrDefault();
            Assert.That(_cityComboBox, Is.Not.Null, "There should be a ComboBox defined in the XAML.");
            _newCustomerGroupBox = _window.FindVisualChildren<GroupBox>().FirstOrDefault();
            Assert.That(_newCustomerGroupBox, Is.Not.Null, "There should be a GroupBox defined in the XAML.");
            _addCustomerButton = _window.GetPrivateFieldValueByName<Button>("AddCustomerButton");
            Assert.That(_addCustomerButton, Is.Not.Null, "There should be a Button with the name AddCustomerButton.");
            _errorTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("ErrorTextBlock");
            Assert.That(_errorTextBlock, Is.Not.Null, "There should be a TextBlock with the name ErrorTextBlock.");
        }

        private Customer TryGetNewCustomerGroupBoxDataContext()
        {
            Customer dataContext = _newCustomerGroupBox.DataContext as Customer;
            Assert.That(dataContext, Is.Not.Null,
                "The DataContext of the GroupBox for adding a new customer, should be a Customer instance.");
            return dataContext;
        }
    }
}
