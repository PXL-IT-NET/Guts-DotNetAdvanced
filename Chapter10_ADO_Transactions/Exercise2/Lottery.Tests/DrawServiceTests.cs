using Guts.Client.Classic;
using Guts.Client.Shared;
using Lottery.Business;
using Lottery.Data.Interfaces;
using Lottery.Domain;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;

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
    public class DrawServiceTests
    {
        private Mock<IDrawRepository> _drawRepositoryMock;
        private DrawService _service;
        private IList<int> _previousNumbers;
        private string _drawServiceClassContent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _drawServiceClassContent = Solution.Current.GetFileContent(@"Lottery.Business\DrawService.cs");
        }

        [SetUp]
        public void Setup()
        {
            _drawRepositoryMock = new Mock<IDrawRepository>();
            _service = new DrawService(_drawRepositoryMock.Object);
            _previousNumbers = null;
        }

        [MonitoredTest("DrawService - CreateDrawFor should generate a correct list of numbers and add a draw using the repository")]
        public void CreateDrawFor_ShouldGenerateACorrectListOfNumbersAndAddADrawUsingTheRepository()
        {
            var someGame = new LotteryGameBuilder().WithId().Build();

            _service.CreateDrawFor(someGame);

            var amountOfRunsToRuleOutRandomness = 10;
            for (int i = 0; i < amountOfRunsToRuleOutRandomness; i++)
            {
                _drawRepositoryMock.Invocations.Clear();
                _service.CreateDrawFor(someGame);
                _drawRepositoryMock.Verify(repo => repo.Add( someGame.Id,It.Is<IList<int>>(numbers => AssertIsValidNumbersList(someGame, numbers))), Times.Once,
                    "The Add method of the repository is not called correctly.");
            }
        }

        [MonitoredTest("DrawService - CreateDrawFor should always generate unique numbers in a draw")]
        public void CreateDrawFor_ShouldAlwaysGenerateUniqueNumbersInADraw()
        {
            var someGame = new LotteryGameBuilder()
                .WithId()
                .WithMaximumNumber(3)
                .WithMaximumNumberOfNumbersInADraw(2)
                .Build();

            var amountOfRunsToRuleOutRandomness = 20;
            bool generatesRandomNumbers = false;
            for (int i = 0; i < amountOfRunsToRuleOutRandomness; i++)
            {
                _drawRepositoryMock.Invocations.Clear();
                _service.CreateDrawFor(someGame);
                bool isDifferent = false;
                _drawRepositoryMock.Verify(repo => repo.Add(someGame.Id, It.Is<IList<int>>(numbers => AssertIsDrawWitUniqueNumbers(numbers, out isDifferent))), Times.Once,
                    "The draw created by the service should be added using the repository.");
                if (isDifferent)
                {
                    generatesRandomNumbers = true;
                }
            }
            Assert.That(generatesRandomNumbers, Is.True, () => "The service does not seem to use random numbers to create a draw.");
        }

        [MonitoredTest("DrawService - Should not have unnecessary comments")]
        public void ShouldNotHaveUnnecessaryComments()
        {
            var syntaxtTree = CSharpSyntaxTree.ParseText(_drawServiceClassContent);
            var root = syntaxtTree.GetRoot();
            var commentCount = root
                .DescendantTrivia()
                .Count(trivia => trivia.Kind() == SyntaxKind.SingleLineCommentTrivia ||
                                 trivia.Kind() == SyntaxKind.MultiLineCommentTrivia);

            Assert.That(commentCount, Is.Zero, () => "Clean up code that is commented out " +
                                                                     "and/or replace comments with meaningful method calls.");
        }

        private bool AssertIsValidNumbersList(LotteryGame game, IList<int> numbers)
        {
            Assert.That(numbers, Is.Not.Null.Or.Empty,  "The 'numbers' list should not be null or empty.");
            Assert.That(numbers.Count, Is.EqualTo(game.NumberOfNumbersInADraw), "An incorrect amount of draw numbers were generated.");

            Assert.That(numbers, Has.All.Matches((int number) => number >= 1),
                 "Not all numbers are greater than or equal to one.");

            Assert.That(numbers, Has.All.Matches((int number) => number <= game.MaximumNumber),
                "Not all draw numbers are less than or equal to the maximum number of the game.");

            return true;
        }

        private bool AssertIsDrawWitUniqueNumbers(IList<int> numbers, out bool hasDifferentNumbersThanPreviousDraw)
        {
            Assert.That(numbers, Is.Not.Empty,  "The 'numbers' list should not be null or empty.");

            var uniqueNumberCount = numbers.Distinct().Count();
            Assert.That(numbers.Count, Is.EqualTo(uniqueNumberCount),
                "The numbers in a draw are not always unique. " +
                "Make sure that you don't generate the same number twice within one draw.");

            hasDifferentNumbersThanPreviousDraw = false;
            if (_previousNumbers != null)
            {
                if (numbers.Count != _previousNumbers.Count)
                {
                    hasDifferentNumbersThanPreviousDraw = true;
                }
                else
                {
                    var index = 0;
                    while (!hasDifferentNumbersThanPreviousDraw && index < numbers.Count)
                    {
                        if (numbers[index] != _previousNumbers[index]) hasDifferentNumbersThanPreviousDraw = true;
                        index++;
                    }
                }
            }

            _previousNumbers = numbers;

            return true;
        }
    }
}