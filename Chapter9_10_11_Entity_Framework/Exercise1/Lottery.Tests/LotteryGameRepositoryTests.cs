﻿using System;
using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Lottery.Domain;
using Lottery.Infrastructure;
using NUnit.Framework;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise01", @"Lottery.Infrastructure\LotteryGameRepository.cs")]
    internal class LotteryGameRepositoryTests : DatabaseTests
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