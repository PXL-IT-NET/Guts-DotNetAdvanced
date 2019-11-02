using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Bank.Tests;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Lottery.Data;
using Lottery.Data.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
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
    internal class LotteryGameRepositoryTests : DatabaseTestsBase
    {
        private LotteryGameRepository _repository;
        private string _repositoryClassContent;
        private Mock<IConnectionFactory> _connectionFactoryMock;
        private SqlConnection _connection;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repositoryClassContent = Solution.Current.GetFileContent(@"Lottery.Data\LotteryGameRepository.cs");
        }

        [SetUp]
        public void Setup()
        {
            _connectionFactoryMock = new Mock<IConnectionFactory>();
            _connection = Cc();
            _connectionFactoryMock.Setup(factory => factory.CreateSqlConnection()).Returns(_connection);

            _repository = new LotteryGameRepository(_connectionFactoryMock.Object);
        }

        [MonitoredTest("LotteryGameRepository - GetAll should create and close a database connection")]
        public void GetAll_ShouldCreateAndCloseConnection()
        {
            AssertConnectionIsCreatedAndClosed(() => _repository.GetAll());
        }

        [MonitoredTest("LotteryGameRepository - GetAll should return all games from the database")]
        public void GetAll_ShouldReturnAllGamesFromDb()
        {
            //Arrange
            var allOriginalGames = GetAllGames();

            //Act
            var retrievedGames = _repository.GetAll();

            //Assert
            Assert.That(retrievedGames, Is.Not.Null, "The method returns null while there are games in the database.");
            Assert.That(retrievedGames.Count, Is.EqualTo(allOriginalGames.Count),
                "Not all games in the database are returned.");

            foreach (var retrievedGame in retrievedGames)
            {
                var matchingOriginal = allOriginalGames.FirstOrDefault(game => game.Id == retrievedGame.Id);

                Assert.That(matchingOriginal, Is.Not.Null,
                    () => "The 'Id' property of one or more games is not correct.");
                Assert.That(retrievedGame.Name, Is.EqualTo(matchingOriginal.Name),
                    () => "The 'Name' property of one or more games is not correct.");
                Assert.That(retrievedGame.NumberOfNumbersInADraw, Is.EqualTo(matchingOriginal.NumberOfNumbersInADraw),
                    () => "The 'NumberOfNumbersInADraw' property of one or more games is not correct.");
                Assert.That(retrievedGame.MaximumNumber, Is.EqualTo(matchingOriginal.MaximumNumber),
                    () => "The 'MaximumNumber' property of one or more games is not correct.");
            }
        }

        [MonitoredTest("LotteryGameRepository - Should not have unnecessary comments")]
        public void ShouldNotHaveUnnecessaryComments()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_repositoryClassContent);
            var root = syntaxTree.GetRoot();
            var commentCount = root
                .DescendantTrivia()
                .Count(trivia => trivia.Kind() == SyntaxKind.SingleLineCommentTrivia ||
                                 trivia.Kind() == SyntaxKind.MultiLineCommentTrivia);

            Assert.That(commentCount, Is.Zero,
                "Clean up code that is commented out and/or replace comments with meaningful method calls.");
        }

        private void AssertConnectionIsCreatedAndClosed(Action action)
        {
            _connectionFactoryMock.Invocations.Clear();

            action.Invoke();

            _connectionFactoryMock.Verify(factory => factory.CreateSqlConnection(), Times.Once,
                "The 'ConnectionFactory' should be used to create a new 'SqlConnection' each time the repository method is called.");
            Assert.That(_connection.State, Is.EqualTo(ConnectionState.Closed), "The created connection is not closed.");
        }
    }
}
