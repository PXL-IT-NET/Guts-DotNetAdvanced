using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Business;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;
using Guts.Client.Classic;
using Guts.Client.Shared;
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
    internal class CustomerValidatorTests
    {
        private Mock<ICityRepository> _cityRepositoryMock;
        private CustomerValidator _validator;
        private readonly Random _random;
        private List<City> _existingCities;
        private int _nonExistingZipCode;

        public CustomerValidatorTests()
        {
            _random = new Random();
        }

        [SetUp]
        public void SetUp()
        {
            _nonExistingZipCode = 99999;
            var zipCode = _random.Next(1, 10000);
            _existingCities = new List<City>()
            {
                new City{Name = Guid.NewGuid().ToString(), ZipCode = zipCode},
                new City{Name = Guid.NewGuid().ToString(), ZipCode = zipCode + 1},
                new City{Name = Guid.NewGuid().ToString(), ZipCode = zipCode + 2}
            };
            _cityRepositoryMock = new Mock<ICityRepository>();
            _cityRepositoryMock.Setup(repo => repo.GetAll()).Returns(_existingCities);
            _validator = new CustomerValidator(_cityRepositoryMock.Object);
        }

        [MonitoredTest("CustomerValidator - IsValid should pass for valid customer")]
        public void IsValid_ShouldPassForValidCustomer()
        {
            //Arrange
            var customer = new CustomerBuilder().WithZipCode(_existingCities.First().ZipCode).Build();

            //Act
            var result = _validator.IsValid(customer);

            //Assert
            Assert.That(result.IsValid, Is.True);
        }

        [MonitoredTest("CustomerValidator - IsValid should fail when customer is null")]
        public void IsValid_ShouldFailWhenCustomerIsNull()
        {
            //Act
            var result = _validator.IsValid(null);

            //Assert
            Assert.That(result.IsValid, Is.False, "Result should be invalid.");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty, "Message should not be empty.");
        }

        [MonitoredTest("CustomerValidator - IsValid should fail on invalid name")]
        public void IsValid_ShouldFailOnInvalidName()
        {
            AssertIsInvalid("", "validName");
            AssertIsInvalid(null, "validName");
            AssertIsInvalid("validFirstName", "");
            AssertIsInvalid("validFirstName", null);
        }

        [MonitoredTest("CustomerValidator - IsValid should fail on non existing zipcode")]
        public void IsValid_ShouldFailOnNonExistingZipcode()
        {
            //Arrange
            var customer = new CustomerBuilder().WithZipCode(_nonExistingZipCode).Build();

            //Act
            var result = _validator.IsValid(customer);

            //Assert
            Assert.That(result.IsValid, Is.False, "Result should be invalid.");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty, "Message should not be empty.");
        }

        private void AssertIsInvalid(string firstName, string name)
        {
            //Arrange
            var customer = new CustomerBuilder().WithZipCode(_existingCities.First().ZipCode).Build();
            customer.FirstName = firstName;
            customer.Name = name;

            //Act
            var result = _validator.IsValid(customer);

            //Assert
            var forMessage = $"for customer with firstname '{firstName ?? "null"}' and name '{name ?? "null"}'.";
            Assert.That(result.IsValid, Is.False, 
                $"Result should be invalid {forMessage}");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty,
                $"Message should not be empty {forMessage}");
        }
    }
}