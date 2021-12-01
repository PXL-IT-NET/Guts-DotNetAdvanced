using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Domain;
using Bank.Infrastructure;
using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.Infrastructure\CityRepository.cs")]
    internal class CityRepositoryTests : DatabaseTests
    {
        [MonitoredTest("CityRepository - GetAllOrderedByZipCode - Should return all cities from the database sorted on zip code")]
        public void GetAllOrderedByZipCode_ShouldReturnAllCitiesFromDatabaseSortedOnZipCode()
        {
            //Arrange
            var expectedCities = new List<City>();
            var originalAmountOfCities = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfCities = context.Set<City>().Count();

                var numberOfNewCities = RandomGenerator.Next(10, 21);
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
                IReadOnlyList<City> allCities = repo.GetAllOrderedByZipCode();

                //Assert
                Assert.That(allCities, Has.Count.EqualTo(originalAmountOfCities + expectedCities.Count), "An invalid amount of cities is returned.");
                Assert.That(expectedCities,
                    Has.All.Matches((City expectedCity) =>
                        allCities.Any(city => expectedCity.ZipCode == city.ZipCode)),
                    "Not all zip codes in the returned cities match a zip code of a city in the database.");

                for (int i = 1; i < allCities.Count; i++)
                {
                    City left = allCities[i - 1];
                    City right = allCities[i];

                    Assert.That(left.ZipCode, Is.LessThan(right.ZipCode), "The cities are not sorted correctly.");
                }
            }
        }
    }
}