using Guts.Client.Classic;
using Guts.Client.Shared;
using Lottery.Business;
using Lottery.Data.Interfaces;
using Lottery.Domain;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise01", @"Lottery.Data\LotteryContext.cs;Lottery.Data\LotteryGameRepository.cs;Lottery.Data\DrawRepository.cs;Lottery.Business\DrawService.cs;Lottery.UI\LotteryWindow.xaml;Lottery.UI\LotteryWindow.xaml.cs;Lottery.UI\Converters\DrawNumbersConverter.cs;")]
    public class DrawServiceTests
    {
        private Mock<IDrawRepository> _drawRepositoryMock;
        private DrawService _service;
        private Draw _previousDraw;
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
            _previousDraw = null;
        }

        [MonitoredTest("DrawService - CreateDrawFor should generate a correct draw and add it to the repository")]
        public void CreateDrawFor_ShouldGenerateACorrectDrawAndAddItToTheRepository()
        {
            var someGame = new LotteryGameBuilder().WithId().Build();
            var now = DateTime.Now;

            _service.CreateDrawFor(someGame);

            var amountOfRunsToRuleOutRandomness = 10;
            for (int i = 0; i < amountOfRunsToRuleOutRandomness; i++)
            {
                _drawRepositoryMock.Invocations.Clear();
                _service.CreateDrawFor(someGame);
                _drawRepositoryMock.Verify(repo => repo.Add(It.Is<Draw>(draw => AssertIsValidDraw(draw, someGame, now))), Times.Once,
                    "The draw created by the service should be added using the repository.");
            }
        }

        [MonitoredTest("DrawService - CreateDrawFor should always generate unique numbers in a draw")]
        public void CreateDrawFor_ShouldAlwaysGenerateUniqueNumbersInADraw()
        {
            var someGame = new LotteryGameBuilder()
                .WithId()
                .WithMaximumNumber(3)
                .WithMaximumNumberOfNumbersInADrawy(2)
                .Build();

            var amountOfRunsToRuleOutRandomness = 20;
            bool generatesRandomNumbers = false;
            for (int i = 0; i < amountOfRunsToRuleOutRandomness; i++)
            {
                _drawRepositoryMock.Invocations.Clear();
                _service.CreateDrawFor(someGame);
                bool isDifferent = false;
                _drawRepositoryMock.Verify(repo => repo.Add(It.Is<Draw>(draw => AssertIsDrawWitUniqueNumbers(draw, out isDifferent))), Times.Once,
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

        private bool AssertIsValidDraw(Draw draw, LotteryGame game, DateTime now)
        {
            Assert.That(draw.LotteryGameId, Is.EqualTo(game.Id), () => "The 'LotteryGameId' of the draw is incorrect.");
            Assert.That(draw.Date, Is.EqualTo(now).Within(10).Seconds,
                () => "The 'Date' of the draw should be set to the current date.");
            Assert.That(draw.DrawNumbers, Is.Not.Empty, () => "The 'DrawNumbers' collection of the draw should be set.");
            Assert.That(draw.DrawNumbers.Count, Is.EqualTo(game.NumberOfNumbersInADraw), () => "An incorrect amount of draw numbers were generated.");

            var drawNumberList = draw.DrawNumbers.ToList();
            var expectedPosition = 1;
            foreach (var drawNumber in drawNumberList)
            {
                Assert.That(drawNumber.Position, Is.EqualTo(expectedPosition), () =>
                    "One of the draw numbers has an unexpected position. " +
                    "The position of the draw numbers should start at 1 and increment for each new draw number");
                expectedPosition++;
            }

            Assert.That(drawNumberList, Has.All.Matches((DrawNumber drawNumber) => drawNumber.Number >= 1),
                () => "Not all draw numbers are greater than or equal to one.");

            Assert.That(drawNumberList, Has.All.Matches((DrawNumber drawNumber) => drawNumber.Number <= game.MaximumNumber),
                () => "Not all draw numbers are less than or equal to the maximum number of the game.");

            return true;
        }

        private bool AssertIsDrawWitUniqueNumbers(Draw draw, out bool hasDifferentNumbersThanPreviousDraw)
        {
            Assert.That(draw.DrawNumbers, Is.Not.Empty, () => "The 'DrawNumbers' collection of the draw should be set.");

            var uniqueNumberCount = draw.DrawNumbers.Select(dn => dn.Number).Distinct().Count();
            Assert.That(draw.DrawNumbers.Count, Is.EqualTo(uniqueNumberCount), () =>
                "The numbers that are drawed are not always unique. " +
                "Make sure that you don't generate the same number twice within one draw.");

            hasDifferentNumbersThanPreviousDraw = false;
            if (_previousDraw != null)
            {
                var currentNumbers = draw.DrawNumbers.Select(dn => dn.Number).ToList();
                var previousNumbers = _previousDraw.DrawNumbers.Select(dn => dn.Number).ToList();
                if (currentNumbers.Count != previousNumbers.Count)
                {
                    hasDifferentNumbersThanPreviousDraw = true;
                }
                else
                {
                    var index = 0;
                    while (!hasDifferentNumbersThanPreviousDraw && index < currentNumbers.Count)
                    {
                        if (currentNumbers[index] != previousNumbers[index]) hasDifferentNumbersThanPreviousDraw = true;
                        index++;
                    }
                }
            }

            _previousDraw = draw;

            return true;
        }
    }
}