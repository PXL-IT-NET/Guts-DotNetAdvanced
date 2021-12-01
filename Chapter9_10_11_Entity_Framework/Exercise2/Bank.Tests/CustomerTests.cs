using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.Domain\Customer.cs;")]
    public class CustomerTests
    {
        private static Random Random = new Random();

        [MonitoredTest("Customer - Constructor - Should initialize account list")]
        public void Constructor_ShouldInitializeAccountList()
        {
            //Act
            Customer customer = new Customer();

            //Assert
            var accounts = customer.TryGetAccounts();
            Assert.That(accounts, Is.Not.Null, "The Accounts property should not be null.");
            Assert.That(accounts.Count(), Is.Zero, "The number of accounts should be zero after construction.");
        }


        [MonitoredTest("Customer - Validate - Valid customer - Should return success")]
        public void Validate_ValidCustomer_ShouldReturnTrue()
        {
            //Arrange
            List<City> validCities = GenerateSomeValidCities();
            City city = validCities[Random.Next(0, validCities.Count)];
            Customer customer = new CustomerBuilder().WithZipCode(city.ZipCode).Build();


            //Act
            Result result = customer.Validate(validCities);

            //Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [MonitoredTest("Customer - Validate - Required property is empty - Should return failure")]
        public void Validate_RequiredPropertyIsEmpty_ShouldReturnFailure()
        {
            List<City> validCities = GenerateSomeValidCities();
            City city = validCities[Random.Next(0, validCities.Count)];

            Customer customer = new CustomerBuilder().WithZipCode(city.ZipCode).WithName(null).Build();
            Assert.That(customer.Validate(validCities).IsSuccess, Is.False, "Should return failure when Name is null.");

            customer = new CustomerBuilder().WithZipCode(city.ZipCode).WithName("").Build();
            Assert.That(customer.Validate(validCities).IsSuccess, Is.False, "Should return failure when Name is an empty string.");

            customer = new CustomerBuilder().WithZipCode(city.ZipCode).WithFirstName(null).Build();
            Assert.That(customer.Validate(validCities).IsSuccess, Is.False, "Should return failure when FirstName is null.");

            customer = new CustomerBuilder().WithZipCode(city.ZipCode).WithFirstName("").Build();
            Assert.That(customer.Validate(validCities).IsSuccess, Is.False, "Should return failure when FirstName is an empty string.");

            customer = new CustomerBuilder().WithZipCode(city.ZipCode).WithAddress(null).Build();
            Assert.That(customer.Validate(validCities).IsSuccess, Is.False, "Should return failure when Address is null.");

            customer = new CustomerBuilder().WithZipCode(city.ZipCode).WithAddress("").Build();
            Assert.That(customer.Validate(validCities).IsSuccess, Is.False, "Should return failure when Address is an empty string.");
        }

        [MonitoredTest("Customer - Validate - Zip code is not valid - Should return failure")]
        public void Validate_ZipCodeIsNotValid_ShouldReturnFalse()
        {
            List<City> validCities = GenerateSomeValidCities();
            int maxZipCode = validCities.Max(c => c.ZipCode);
            int invalidZipCode = maxZipCode + 1;

            Customer customer = new CustomerBuilder().WithZipCode(invalidZipCode).Build();
            Assert.That(customer.Validate(validCities).IsSuccess, Is.False,
                "Should return failure when none of the valid cities has the zipcode of the customer.");
        }

        private List<City> GenerateSomeValidCities()
        {
            var validCities = new List<City>();

            for (int i = 0; i < Random.Next(3,11); i++)
            {
                validCities.Add(new CityBuilder().Build());
            }

            return validCities;
        }
    }
}