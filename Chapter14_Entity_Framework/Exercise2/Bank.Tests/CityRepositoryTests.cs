using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Data;
using Bank.Data.DomainClasses;
using Guts.Client.Classic;
using Guts.Client.Shared;
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
    internal class CityRepositoryTests : DatabaseTests
    {
        [MonitoredTest("CityRepository - GetAll should return all cities from the database")]
        public void GetAll_ShouldReturnAllCitiesFromDatabase()
        {
            //Arrange
            var expectedCities = new List<City>();
            var originalAmountOfCities = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfCities = context.Set<City>().Count();

                var numberOfNewCities = RandomGenerator.Next(3, 11);
                while (expectedCities.Count < numberOfNewCities)
                {
                    var existingCity = new City
                    {
                        Name = Guid.NewGuid().ToString(), 
                        ZipCode = RandomGenerator.Next(10000, 100000)
                    };
                    if (expectedCities.All(c => c.ZipCode != existingCity.ZipCode))
                    {
                        expectedCities.Add(existingCity);
                    }
                }

                context.AddRange(expectedCities);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new CityRepository(context);

                //Act
                var allCities = repo.GetAll();

                //Assert
                Assert.That(allCities, Has.Count.EqualTo(originalAmountOfCities + expectedCities.Count));
                Assert.That(expectedCities,
                    Has.All.Matches((City expectedCity) =>
                        allCities.Any(city => expectedCity.ZipCode == city.ZipCode)));
            }
        }
    }
}