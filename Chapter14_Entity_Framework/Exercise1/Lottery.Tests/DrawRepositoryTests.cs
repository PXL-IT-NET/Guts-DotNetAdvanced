using System;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Lottery.Data;
using Lottery.Domain;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise01", @"Lottery.Data\LotteryContext.cs;Lottery.Data\LotteryGameRepository.cs;Lottery.Data\DrawRepository.cs;Lottery.Business\DrawService.cs;Lottery.UI\LotteryWindow.xaml;Lottery.UI\LotteryWindow.xaml.cs;Lottery.UI\Converters\DrawNumbersConverter.cs;")]
    public class DrawRepositoryTests : DatabaseTests
    {
        [MonitoredTest("DrawRepository - Find should return all draws of a game when there are no date limits")]
        public void Find_ShouldReturnAllDrawsOfAGameWhenThereAreNoDateLimits()
        {
            //Arrange
            var someGame = new LotteryGameBuilder().WithRandomDraws(5, 10).Build();
            var someOtherGame = new LotteryGameBuilder().WithRandomDraws(5, 10).Build();

            using (var context = CreateDbContext())
            {
                context.Add(someGame);
                context.Add(someOtherGame);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new DrawRepository(context);

                //Act
                var draws = repo.Find(someGame.Id, null, null);

                //Assert
                Assert.That(draws, Is.Not.Null, () => "No draws are returned.");
                Assert.That(draws.Count, Is.Not.GreaterThan(someGame.Draws.Count), () => "Too much draws are returned.");
                Assert.That(draws.Count, Is.EqualTo(someGame.Draws.Count), () => "All the draws of a game should be returned.");
                Assert.That(draws, Has.All.Matches((Draw draw) => draw.LotteryGameId == someGame.Id),
                    () =>
                        "Only draws of the lottery game with an 'Id' that matches the 'lotterGameId' parameter, should be returned.");
            }
        }

        [MonitoredTest("DrawRepository - Find should only return draws after the from date")]
        public void Find_ShouldOnlyReturnDrawsAfterFromDate()
        {
            //Arrange
            var fromDate = DateTime.Now.AddDays(-1);
            var someGame = new LotteryGameBuilder().WithDrawsAroundDateRange(fromDate, null).Build();

            using (var context = CreateDbContext())
            {
                context.Add(someGame);
                context.SaveChanges();
            }

            var expectedDraws = someGame.Draws.Where(d => d.Date >= fromDate).OrderBy(d => d.Date).ToList();

            TestFindForDateRange(someGame.Id, fromDate, null, expectedDraws);
        }

        [MonitoredTest("DrawRepository - Find should only return draws before the until date")]
        public void Find_ShouldOnlyReturnDrawsBeforeUntilDate()
        {
            //Arrange
            var untilDate = DateTime.Now.AddDays(-1);
            var someGame = new LotteryGameBuilder().WithDrawsAroundDateRange(null, untilDate).Build();

            using (var context = CreateDbContext())
            {
                context.Add(someGame);
                context.SaveChanges();
            }

            var expectedDraws = someGame.Draws.Where(d => d.Date <= untilDate).OrderBy(d => d.Date).ToList();

            TestFindForDateRange(someGame.Id, null, untilDate, expectedDraws);
        }

        [MonitoredTest("DrawRepository - Find should only return draws between a date range")]
        public void Find_ShouldOnlyReturnDrawsBetweenDateRange()
        {
            //Arrange
            var fromDate = DateTime.Now.AddDays(-10);
            var untilDate = DateTime.Now.AddDays(-1);
            var someGame = new LotteryGameBuilder().WithDrawsAroundDateRange(fromDate, untilDate).Build();

            using (var context = CreateDbContext())
            {
                context.Add(someGame);
                context.SaveChanges();
            }

            var expectedDraws = someGame.Draws.Where(d => d.Date >= fromDate && d.Date <= untilDate).OrderBy(d => d.Date).ToList();

            TestFindForDateRange(someGame.Id, fromDate, untilDate, expectedDraws);
        }

        [MonitoredTest("DrawRepository - Find should include the draw numbers of the returned draws")]
        public void Find_ShouldIncludeTheDrawNumbersOfTheReturnedDraws()
        {
            //Arrange
            var someGame = new LotteryGameBuilder().WithRandomDraws(1, 1).Build();

            using (var context = CreateDbContext())
            {
                context.Add(someGame);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new DrawRepository(context);

                //Act
                var draws = repo.Find(someGame.Id, null, null);

                //Assert
                Assert.That(draws, Is.Not.Null, () => "No draws are returned.");
                var firstDraw = draws.FirstOrDefault();
                Assert.That(firstDraw, Is.Not.Null, () => "No draws are returned.");

                Assert.That(firstDraw.DrawNumbers, Is.Not.Empty, () => "The draw numbers of the draws are not returned from the database.");
            }
        }

        [MonitoredTest("DrawRepository - Find should contain less than 450 characters")]
        public void Find_ShouldContainLessThan450Characters()
        {
            var code = Solution.Current.GetFileContent(@"Lottery.Data\DrawRepository.cs");
            var syntaxtTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxtTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("Find"));
            Assert.That(method, Is.Not.Null,
                () => "Could not find the 'Find' method. You may have accidentially deleted or renamed it?");

            var numberOfCharacters = method.Body.FullSpan.Length;
            Assert.That(numberOfCharacters, Is.LessThan(450), () => $"The find method contains too much characters ({numberOfCharacters}). Try to find a more elegant solution. " +
                                                                    "Tip 1: you only need to code one Linq query. " +
                                                                    "Tip 2: in SQL you can filter on a parameter that may be null like this: select * from dbo.table t where (@p = null or t.column = @p).");

            var numberOfifStatements = method.Body.Statements.OfType<IfStatementSyntax>().Count();
            Assert.That(numberOfifStatements, Is.Zero, () => "An if-statement is found in the method body. " +
                                                             "You should be able to code this method without using if-statements.");
        }

        [MonitoredTest("DrawRepository - Add should add a draw to the database")]
        public void Add_ShouldAddADrawToTheDatabase()
        {
            //Arrange
            var someGame = new LotteryGameBuilder().Build();
            using (var context = CreateDbContext())
            {
                context.Add(someGame);
                context.SaveChanges();
            }

            var newDraw = new DrawBuilder().WithLotteryGameId(someGame.Id).WithRandomDrawNumbers(1, 1).Build();

            using (var context = CreateDbContext())
            {
                var repo = new DrawRepository(context);

                //Act
                repo.Add(newDraw);
            }

            using (var context = CreateDbContext())
            {
                //Assert
                var savedDraw = context.Set<Draw>().FirstOrDefault(draw => draw.Id == newDraw.Id);
                Assert.That(savedDraw, Is.Not.Null, () => "Cannot find the added draw in the database.");
            }
        }

        [MonitoredTest("DrawRepository - Add should throw an ArgumentException when the draw contains no draw numbers")]
        public void Add_ShouldThrowArgumentExceptionWhenTheDrawContainsNoDrawNumbers()
        {
            var newDraw = new DrawBuilder().Build();

            using (var context = CreateDbContext())
            {
                var repo = new DrawRepository(context);

                newDraw.DrawNumbers = null;
                Assert.That(() => repo.Add(newDraw), Throws.ArgumentException,
                    () => "Should throw an 'ArgumentException' when 'DrawNumbers' is null.");

                newDraw.DrawNumbers = new List<DrawNumber>();
                Assert.That(() => repo.Add(newDraw), Throws.ArgumentException,
                    () => "Should throw an 'ArgumentException' when 'DrawNumbers' is empty.");
            }
        }

        private void TestFindForDateRange(int gameId, DateTime? fromDate, DateTime? untilDate, IList<Draw> expectedDraws)
        {
            using (var context = CreateDbContext())
            {
                var repo = new DrawRepository(context);

                //Act
                var draws = repo.Find(gameId, fromDate, untilDate);

                //Assert
                Assert.That(draws, Is.Not.Empty, () => "No draws are returned.");

                Assert.That(draws, Has.Count.LessThanOrEqualTo(expectedDraws.Count),
                    () => "One or more draws that are out of range are returned.");

                Assert.That(draws, Has.Count.EqualTo(expectedDraws.Count),
                    () => "Not all draws that match the date range are returned. " +
                          "Make sure you also include the draws exactly on the from date or until date.");
            }
        }
    }
}