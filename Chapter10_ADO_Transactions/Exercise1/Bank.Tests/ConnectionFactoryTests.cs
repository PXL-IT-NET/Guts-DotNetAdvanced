using System.Configuration;
using Bank.Data;
using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise01",
        @"Bank.Data\DomainClasses\Account.cs;Bank.Data\AccountRepository.cs;Bank.Data\CityRepository.cs;Bank.Data\ConnectionFactory.cs;Bank.Data\CustomerRepository.cs;Bank.UI\AccountsWindow.xaml;Bank.UI\AccountsWindow.xaml.cs;Bank.UI\CustomersWindow.xaml;Bank.UI\CustomersWindow.xaml.cs;Bank.UI\TransferWindow.xaml;Bank.UI\TransferWindow.xaml.cs")]
    public class ConnectionFactoryTests
    {
        [MonitoredTest("ConnectionFactory - Should use the connection string in app.config")]
        public void CreateSqlConnection_ShouldUseConnectionStringInAppConfig()
        {
            //Arrange
            var factory = new ConnectionFactory();
            var connenctionStringFromAppConfig = ConfigurationManager.ConnectionStrings["BankConnection"].ConnectionString;

            //Act
            var connection = factory.CreateSqlConnection();

            //Assert
            Assert.That(connection, Is.Not.Null, () => "No instance of SqlConnection is returned.");
            Assert.That(connection.ConnectionString, Is.EqualTo(connenctionStringFromAppConfig),
                () => "The connection string with name 'BankConnection' in app.config should be used.");
        }
    }
}