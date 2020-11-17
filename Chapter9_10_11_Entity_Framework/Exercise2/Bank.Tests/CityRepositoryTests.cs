using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Data;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02",
        @"Bank.Data\CityRepository.cs")]
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