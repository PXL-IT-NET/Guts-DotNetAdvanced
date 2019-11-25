using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bank.Tests;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Lottery.Data;
using Lottery.Data.Interfaces;
using Lottery.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    internal class DrawRepositoryTests : DatabaseTestsBase
    {
        private DrawRepository _repository;
        private Random _random;
        private Mock<IConnectionFactory> _connectionFactoryMock;
        private SqlConnection _connection;
        private SyntaxNode _classSyntaxTreeRoot;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var repositoryClassContent = Solution.Current.GetFileContent(@"Lottery.Data\DrawRepository.cs");
            var codeWithoutComments = CodeCleaner.StripComments(repositoryClassContent);
            var syntaxTree = CSharpSyntaxTree.ParseText(codeWithoutComments);
            _classSyntaxTreeRoot = syntaxTree.GetRoot();
        }

        [SetUp]
        public void Setup()
        {
            _connectionFactoryMock = new Mock<IConnectionFactory>();
            _connection = Cc();
            _connectionFactoryMock.Setup(factory => factory.CreateSqlConnection()).Returns(_connection);

            _repository = new DrawRepository(_connectionFactoryMock.Object);

            _random = new Random();
        }

        [MonitoredTest("DrawRepository - Add should create and close a database connection")]
        public void Add_ShouldCreateAndCloseConnection()
        {
            GenerateGameIdAndNumbers(out int gameId, out List<int> numbers);

            AssertConnectionIsCreatedAndClosed(() => _repository.Add(gameId, numbers));
        }

        [MonitoredTest("DrawRepository - Add should add a draw to the database")]
        public void Add_ShouldAddADrawToTheDatabase()
        {
            //Arrange
            GenerateGameIdAndNumbers(out int gameId, out List<int> numbers);

            var originalDraws = GetAllDraws().Where(d => d.LotteryGameId == gameId).ToList();

            //Act
            _repository.Add(gameId, numbers);

            //Assert
            var draws = GetAllDraws().Where(d => d.LotteryGameId == gameId).ToList();
            Assert.That(draws.Count, Is.EqualTo(originalDraws.Count + 1), "No draw was added.");

            var savedDraw = draws.Last();
            Assert.That(savedDraw.Date, Is.EqualTo(DateTime.Now).Within(5).Seconds,
                "The date of the draw must be 'Now'.");

            var savedDrawNumbers = GetAllDrawNumbers().Where(dn => dn.DrawId == savedDraw.Id).OrderBy(dn => dn.Position)
                .ToList();
            Assert.That(savedDrawNumbers.Count, Is.EqualTo(numbers.Count),
                "For each number passed to the 'Add' method a 'DrawNumber' should be saved.");

            for (int i = 0; i < numbers.Count; i++)
            {
                Assert.That(savedDrawNumbers[i].Position, Is.EqualTo(i + 1),
                    "The positions of the ''DrawNumbers'' must start at 1 and increment by 1.");
                Assert.That(savedDrawNumbers[i].Number, Is.EqualTo(numbers[i]), $"Unexpected number at position {i + 1}.");
            }
        }

        [MonitoredTest("DrawRepository - Add should use a transaction")]
        public void Add_ShouldUseATransaction()
        {
            var method = _classSyntaxTreeRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("Add"));
            Assert.That(method, Is.Not.Null,
                () => "Could not find the 'Add' method. You may have accidentally deleted or renamed it?");

            var methodCode = method.ToString();

            Assert.That(methodCode, Contains.Substring(".BeginTransaction();"),
                () => "No method call found in the 'Add' method that begins a transaction.");
            Assert.That(methodCode, Contains.Substring(".Transaction ="),
                () => "No code found in the 'Add' method that links the transaction to the commands.");
            Assert.That(methodCode, Contains.Substring(".Commit();"),
                () => "No code found in the 'Add' method that commits the transaction.");
            Assert.That(methodCode, Contains.Substring(".Rollback();"),
                () => "No code found in the 'Add' method that rolls back the transaction.");
        }

        [MonitoredTest("DrawRepository - Add should use parameters")]
        public void Add_ShouldUseParameters()
        {
            var method = _classSyntaxTreeRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("Add"));
            Assert.That(method, Is.Not.Null,
                 "Could not find the 'Add' method. You may have accidentally deleted or renamed it?");

            var methodCodeBuilder = new StringBuilder();
            methodCodeBuilder.Append(method);

            //include code of private methods that are called in the Add method
            var privateMethodNames = method.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocation => invocation.Expression is IdentifierNameSyntax)
                .Select(invocation => invocation.Expression.ToString())
                .ToList();
            var privateMethods = _classSyntaxTreeRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(md => privateMethodNames.Any(name => name == md.Identifier.ValueText)).ToList();

            foreach (var privateMethod in privateMethods)
            {
                methodCodeBuilder.AppendLine();
                methodCodeBuilder.Append(privateMethod);
            }

            var methodCode = methodCodeBuilder.ToString();
            var regex = new Regex(@"\.Parameters\.Add");

            Assert.That(regex.Matches(methodCode), Has.Count.GreaterThanOrEqualTo(5),
                 "At least 5 parameters should be used. 2 parameters to insert the draw and 3 parameters for each draw number.");
        }

        [MonitoredTest("DrawRepository - Add should rollback transaction when something goes wrong")]
        public void Add_ShouldRollbackTransactionWhenSomethingGoesWrong()
        {
            try
            {
                //Arrange
                ExecuteScript(
                    "ALTER TABLE dbo.DrawNumbers ADD CONSTRAINT CK_DrawNumbers_Number_Range CHECK (Number > 0 AND Number < 999)");
                var game = GetAllGames().First(g => g.Name == "Normal");
                var originalDraws = GetAllDraws().Where(d => d.LotteryGameId == game.Id).ToList();
                var originalDrawNumbers = GetAllDrawNumbers().ToList();

                var numbers = new List<int>();
                for (int i = 0; i < game.NumberOfNumbersInADraw - 2; i++)
                {
                    numbers.Add(_random.Next(1, game.MaximumNumber + 1));
                }

                numbers.Add(-1 * _random.Next(1, game.MaximumNumber + 1));
                numbers.Add(_random.Next(1000, int.MaxValue));

                try
                {
                    //Act
                    _repository.Add(game.Id, numbers);
                }
                catch (Exception)
                {
                }

                //Assert
                var draws = GetAllDraws().Where(d => d.LotteryGameId == game.Id).ToList();
                Assert.That(draws.Count, Is.EqualTo(originalDraws.Count),
                    "No draw should be saved when something goes wrong with saving the draw numbers.");

                var drawNumbers = GetAllDrawNumbers().ToList();
                Assert.That(drawNumbers.Count, Is.EqualTo(originalDrawNumbers.Count),
                    "No draw numbers should be saved when something goes wrong with saving one of the other draw numbers.");
            }
            finally
            {
                ExecuteScript("ALTER TABLE dbo.DrawNumbers DROP CONSTRAINT CK_DrawNumbers_Number_Range");
            }
        }

        [MonitoredTest("DrawRepository - Add should throw an ArgumentException when lotteryGameId is invalid")]
        public void Add_ShouldThrowArgumentExceptionWhenLotteryGameIdIsInvalid()
        {
            //Arrange
            var numbers = new List<int> { 1, 2, 3 };
            var invalidGameId = -1 * _random.Next(0, int.MaxValue);

            //Act + Assert
            Assert.That(() => _repository.Add(invalidGameId, numbers), Throws.ArgumentException,
                $"Should throw an 'ArgumentException' when 'LotteryGameId' is {invalidGameId}.");
        }

        [MonitoredTest("DrawRepository - Add should throw an ArgumentException no numbers are supplied")]
        public void Add_ShouldThrowArgumentExceptionWhenNoNumbersAreSupplied()
        {
            //Arrange
            var invalidNumbers = new List<int>();
            var gameId = _random.Next(1, int.MaxValue);

            //Act + Assert
            Assert.That(() => _repository.Add(gameId, invalidNumbers), Throws.ArgumentException,
                "Should throw an 'ArgumentException' when the list of numbers is empty.");

            Assert.That(() => _repository.Add(gameId, null), Throws.ArgumentException,
                "Should throw an 'ArgumentException' when the list of numbers is null.");
        }

        [MonitoredTest("DrawRepository - Find should create and close a database connection")]
        public void Find_ShouldCreateAndCloseConnection()
        {
            var game = GetAllGames().First(g => g.Name == "Normal");

            AssertConnectionIsCreatedAndClosed(() => _repository.Find(game.Id, null, null));
        }

        [MonitoredTest("DrawRepository - Find should return all draws of a game when there are no date limits")]
        public void Find_ShouldReturnAllDrawsOfAGameWhenThereAreNoDateLimits()
        {
            //Arrange
            var game = GetAllGames().First(g => g.Name == "Normal");

            //Act
            var draws = _repository.Find(game.Id, null, null);

            //Assert
            var expectedDraws = GetAllDraws().Where(d => d.LotteryGameId == game.Id).ToList();
            Assert.That(draws, Is.Not.Null, "No draws are returned.");
            Assert.That(draws.Count, Is.Not.GreaterThan(expectedDraws.Count), "Too much draws are returned.");
            Assert.That(draws.Count, Is.EqualTo(expectedDraws.Count), "All the draws of a game should be returned.");
            Assert.That(draws, Has.All.Matches((Draw draw) => draw.LotteryGameId == game.Id),
                () =>
                    "Only draws of the lottery game with an 'Id' that matches the 'lotterGameId' parameter, should be returned.");
        }

        [MonitoredTest("DrawRepository - Find should only return draws after the from date")]
        public void Find_ShouldOnlyReturnDrawsAfterFromDate()
        {
            var game = GetAllGames().First(g => g.Name == "Normal");
            var fromDate = new DateTime(2018, 11, 2).AddDays(_random.Next(0, 365)); //should filter out at least one draw (but not all)

            var expectedDraws = GetAllDraws().Where(d => d.LotteryGameId == game.Id && d.Date >= fromDate).OrderBy(d => d.Date).ToList();

            TestFindForDateRange(game.Id, fromDate, null, expectedDraws);
        }

        [MonitoredTest("DrawRepository - Find should only return draws before the until date")]
        public void Find_ShouldOnlyReturnDrawsBeforeUntilDate()
        {
            var game = GetAllGames().First(g => g.Name == "Normal");
            var untilDate = new DateTime(2018, 11, 2).AddDays(_random.Next(0, 365)); //should filter out at least one draw (but not all)

            var expectedDraws = GetAllDraws().Where(d => d.LotteryGameId == game.Id && d.Date <= untilDate).OrderBy(d => d.Date).ToList();

            TestFindForDateRange(game.Id, null, untilDate, expectedDraws);
        }

        [MonitoredTest("DrawRepository - Find should only return draws between a date range")]
        public void Find_ShouldOnlyReturnDrawsBetweenDateRange()
        {
            var game = GetAllGames().First(g => g.Name == "Normal");
            var fromDate = new DateTime(2018, 10, 1).AddDays(_random.Next(0, 365));
            var untilDate = fromDate.AddDays(_random.Next(200, 365));

            var expectedDraws = GetAllDraws().Where(d => d.LotteryGameId == game.Id && d.Date >= fromDate && d.Date <= untilDate).OrderBy(d => d.Date).ToList();

            TestFindForDateRange(game.Id, fromDate, untilDate, expectedDraws);
        }

        [MonitoredTest("DrawRepository - Find should return an empty list when all draws are outside the date range")]
        public void Find_ShouldReturnAnEmptyListWhenAllDrawsAreOutsideTheDateRange()
        {
            var game = GetAllGames().First(g => g.Name == "Normal");
            var fromDate = new DateTime(1900, 1, 1);
            var untilDate = fromDate.AddDays(_random.Next(10, 365));

            var draws = _repository.Find(game.Id, fromDate, untilDate);

            Assert.That(draws, Is.Not.Null, "The returned list should not be null.");
            Assert.That(draws, Is.Empty, "The returned list is not empty.");
        }

        [MonitoredTest("DrawRepository - Find should include the draw numbers of the returned draws")]
        public void Find_ShouldIncludeTheDrawNumbersOfTheReturnedDraws()
        {
            //Arrange
            var game = GetAllGames().First(g => g.Name == "Normal");

            //Act
            var draws = _repository.Find(game.Id, null, null);

            //Assert
            Assert.That(draws, Is.Not.Null.Or.Empty, "No draws are returned.");

            var draw = draws[_random.Next(0, draws.Count)];
            var expectedDrawNumbers = GetAllDrawNumbers().Where(dn => dn.DrawId == draw.Id).ToList();

            Assert.That(draw.DrawNumbers, Is.Not.Empty, "The draw numbers of the draws are missing.");

            foreach (var drawNumber in draw.DrawNumbers)
            {
                Assert.That(drawNumber.DrawId, Is.EqualTo(draw.Id), "DrawId of one of the DrawNumbers is not correct.");
                var matchingDrawNumber = expectedDrawNumbers.FirstOrDefault(dn => dn.Number == drawNumber.Number);
                Assert.That(matchingDrawNumber, Is.Not.Null, "One of the DrawNumbers has an unexpected number.");
                Assert.That(drawNumber.Position, Is.EqualTo(matchingDrawNumber.Position),
                    "One of the DrawNumbers has an unexpected position.");
            }
        }

        [MonitoredTest("DrawRepository - Find should use one SQL command that does the job")]
        public void Find_ShouldUseOneSqlCommandThatDoesTheJob()
        {
            var method = _classSyntaxTreeRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("Find"));
            Assert.That(method, Is.Not.Null, "Could not find the 'Find' method. You may have accidentally deleted or renamed it?");

            var methodCode = method.ToString();

            Assert.That(methodCode, Does.Match(@"SELECT\s[\w\W]+FROM[\w\W]+JOIN"),
                "No string found that constructs a SELECT statement with a JOIN.");

            Assert.That(methodCode, Does.Match(@"SELECT\s[\w\W]+FROM[\w\W]+JOIN[\w\W]+WHERE"),
                "The string that constructs the SELECT statement should also use a WHERE filter.");

            var executeReaderCalls = method.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Count(ma => ma.Name.ToString() == "ExecuteReader");

            Assert.That(executeReaderCalls, Is.EqualTo(1), "'ExecuteReader' should be called exactly once (no more, no less).");
        }

        [MonitoredTest("DrawRepository - Find should use parameters")]
        public void Find_ShouldUseParameters()
        {
            var method = _classSyntaxTreeRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("Find"));
            Assert.That(method, Is.Not.Null,
                () => "Could not find the 'Find' method. You may have accidentally deleted or renamed it?");

            var methodCode = method.ToString();

            var regex = new Regex(@"\.Parameters\.Add");

            Assert.That(regex.Matches(methodCode), Has.Count.GreaterThanOrEqualTo(3),
                "At least 3 parameters should be used (Id of the game, from date, until date).");
        }

        private void TestFindForDateRange(int gameId, DateTime? fromDate, DateTime? untilDate, IList<Draw> expectedDraws)
        {
            //Act
            var draws = _repository.Find(gameId, fromDate, untilDate);

            //Assert
            Assert.That(draws, Is.Not.Empty, "No draws are returned.");

            Assert.That(draws, Has.Count.LessThanOrEqualTo(expectedDraws.Count),
                () => "One or more draws that are out of range are returned.");

            Assert.That(draws, Has.Count.EqualTo(expectedDraws.Count),
                () => "Not all draws that match the date range are returned. " +
                      "Make sure you also include the draws exactly on the from date or until date.");
        }

        private void GenerateGameIdAndNumbers(out int gameId, out List<int> numbers)
        {
            var game = GetAllGames().First(g => g.Name == "Normal");
            gameId = game.Id;
            numbers = new List<int>();
            var step = _random.Next(1, 5);
            for (int i = 1; i <= game.NumberOfNumbersInADraw; i++)
            {
                numbers.Add(i * step);
            }
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