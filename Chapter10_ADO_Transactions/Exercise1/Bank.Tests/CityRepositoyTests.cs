using System.Linq;
using Bank.Data;
using Bank.Data.Interfaces;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise01",
        @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    internal class CityRepositoyTests : DatabaseTestsBase
    {
        private CityRepository _repository;

        [SetUp]
        public void Setup()
        {
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            connectionFactoryMock.Setup(factory => factory.CreateSqlConnection()).Returns(Cc);

            _repository = new CityRepository(connectionFactoryMock.Object);
        }

        [MonitoredTest("CityRepository - GetAll should return all cities from the database")]
        public void GetAll_ShouldReturnAllCitiesFromDatabase()
        {
            //Arrange
            var allOriginalCities = GetAllCities();

            //Act
            var retrievedCities = _repository.GetAll();

            //Assert
            Assert.That(retrievedCities, Is.Not.Null,
                () => "The method returns null while there are cities in the database.");
            Assert.That(retrievedCities.Count, Is.EqualTo(allOriginalCities.Count),
                () => "Not all cities in the database are returned.");

            foreach (var retrievedCity in retrievedCities)
            {
                var matchingOriginal =
                    allOriginalCities.FirstOrDefault(city =>
                        city.ZipCode == retrievedCity.ZipCode);

                Assert.That(matchingOriginal, Is.Not.Null,
                    () => "The 'ZipCode' property of one or more cities is not correct.");
                Assert.That(retrievedCity.Name, Is.EqualTo(matchingOriginal.Name),
                    () => "The 'Name' property of one or more cities is not correct.");
            }
        }
    }
}