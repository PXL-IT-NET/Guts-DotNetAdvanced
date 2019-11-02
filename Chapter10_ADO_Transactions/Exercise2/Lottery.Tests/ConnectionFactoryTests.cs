using System.Configuration;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Lottery.Data;
using NUnit.Framework;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise02",
        @"Lottery.Data\ConnectionFactory.cs;
Lottery.Data\LotteryGameRepository.cs;
Lottery.Data\DrawRepository.cs;
Lottery.Business\DrawService.cs;
Lottery.UI\LotteryWindow.xaml;
Lottery.UI\LotteryWindow.xaml.cs;
Lottery.UI\Converters\DrawNumbersConverter.cs;")]
    public class ConnectionFactoryTests
    {
        [MonitoredTest("ConnectionFactory - Should use the connection string in app.config")]
        public void CreateSqlConnection_ShouldUseConnectionStringInAppConfig()
        {
            //Arrange
            var factory = new ConnectionFactory();
            var connenctionStringFromAppConfig = ConfigurationManager.ConnectionStrings["LotteryConnection"].ConnectionString;

            //Act
            var connection = factory.CreateSqlConnection();

            //Assert
            Assert.That(connection, Is.Not.Null, "No instance of SqlConnection is returned.");
            Assert.That(connection.ConnectionString, Is.EqualTo(connenctionStringFromAppConfig),
                () => "The connection string with name 'LotteryConnection' in app.config should be used.");
        }
    }
}