using System;
using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Lottery.Data;
using Lottery.Domain;
using NUnit.Framework;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise01", @"Lottery.Data\LotteryContext.cs;Lottery.Data\LotteryGameRepository.cs;Lottery.Data\DrawRepository.cs;Lottery.Business\DrawService.cs;Lottery.UI\LotteryWindow.xaml;Lottery.UI\LotteryWindow.xaml.cs;Lottery.UI\Converters\DrawNumbersConverter.cs;")]
    public class LotteryGameRepositoryTests : DatabaseTests
    {
        [MonitoredTest("LotteryGameRepository - GetAll should return all games from the database")]
        public void GetAll_ShouldReturnAllGamesFromDb()
        {
            //Arrange
            var someGame = new LotteryGame
            {
                Name = Guid.NewGuid().ToString(),
                NumberOfNumbersInADraw = 6,
                MaximumNumber = 45

            };
            var originalAmountOfGames = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfGames = context.Set<LotteryGame>().Count();

                context.Add(someGame);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new LotteryGameRepository(context);

                //Act
                var allGames = repo.GetAll();

                //Assert
                Assert.That(allGames, Has.Count.EqualTo(originalAmountOfGames + 1), () => "Not all games in the database are returned.");
                var expectedGame = allGames.FirstOrDefault(game => game.Name == someGame.Name);
                Assert.That(expectedGame, Is.Not.Null, () => "Not all games in the database are returned.");
            }
        }
    }
}