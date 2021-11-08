using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using HeroApp.AppLogic;
using HeroApp.AppLogic.Contracts;
using HeroApp.Domain.Contracts;
using Moq;
using NUnit.Framework;

namespace HeroApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise01",
        @"HeroApp.AppLogic\BattleService.cs;HeroApp.AppLogic\Contracts\IBattleService.cs")]
    public class BattleServiceTests
    {
        private Type _battleServiceType;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _battleServiceType = typeof(BattleService);
        }

        [MonitoredTest("IBattleService - Should not have changed interface")]
        public void ShouldNotHaveChangedIBattleService()
        {
            var filePath = @"HeroApp.AppLogic\Contracts\IBattleService.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("B5-B6-94-9C-8A-66-3F-90-19-DB-21-A3-39-DC-B6-1D"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("BattleService - Should implement IBattleService")]
        public void ShouldImplementIBattleService()
        {
            Assert.That(typeof(IBattleService).IsAssignableFrom(_battleServiceType), Is.True);
        }

        [MonitoredTest("BattleService - Should only be visible to the application logic layer")]
        public void ShouldOnlyBeVisibleToTheAppLogicLayer()
        {
            Assert.That(_battleServiceType.IsNotPublic,
                "Only IBattleService should be visible to the other layers. The BattleService class itself can be encapsulated in the application logic layer.");
        }

        [MonitoredTest("BattleService - Should have a constructor that accepts a hero repository and a battle factory")]
        public void ShouldHaveAConstructorThatAcceptsAHeroRepositoryAndABattleFactory()
        {
            ConstructorInfo[] constructors = _battleServiceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");

            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2), "The constructor should have 2 parameters.");
            Assert.That(parameters.Any(p => p.ParameterType == typeof(IHeroRepository)), Is.True,
                "One of the constructor parameters should be of type IHeroRepository.");
            Assert.That(parameters.Any(p => p.ParameterType == typeof(IBattleFactory)), Is.True,
                "One of the constructor parameters should be of type IBattleFactory.");
        }


        [MonitoredTest("BattleService - SetupRandomBattle - Should randomly pick 2 fighters from all heroes")]
        public void SetupRandomBattle_ShouldRandomlyPick2FightersFromAllHeroes()
        {
            int amountOfHeroes = 5;
            var indexCountDictionary = new Dictionary<int, int>();
            for (int i = 0; i < amountOfHeroes; i++)
            {
                indexCountDictionary.Add(i, 0);
            }

            int amountOfBattles = 100;
            for (int i = 0; i < amountOfBattles; i++)
            {
                (int, int) heroIndexes = AssertRandomBattleSetup(amountOfHeroes);
                indexCountDictionary[heroIndexes.Item1] = indexCountDictionary[heroIndexes.Item1] + 1;
                indexCountDictionary[heroIndexes.Item2] = indexCountDictionary[heroIndexes.Item2] + 1;
            }

            int differentHeroCount = indexCountDictionary.Count(kv => kv.Value > 0);
            Assert.That(differentHeroCount, Is.EqualTo(amountOfHeroes),
                $"When there are {amountOfHeroes} heroes and {amountOfBattles} are created, each heroe should have been picked at least once.");

        }

        [MonitoredTest("BattleService - SetupRandomBattle - Only one hero available - Should throw InvalidOperationException")]
        public void SetupRandomBattle_OnlyOneHero_ShouldThrowInvalidOperationException()
        {
            var battleServiceBuilder = new BattleServiceBuilder().WithHeroes(1);
            IBattleService service = battleServiceBuilder.Build();
            Assert.That(() => service.SetupRandomBattle(), Throws.InvalidOperationException);
        }

        private static (int, int) AssertRandomBattleSetup(int amountOfHeroes)
        {
            //Arrange
            int index1 = -1, index2 = -1;
            var battleServiceBuilder = new BattleServiceBuilder().WithHeroes(amountOfHeroes);
            IBattle createdBattle = null;
            battleServiceBuilder.BattleFactoryMock
                .Setup(factory => factory.CreateNewBattle(It.IsAny<IHero>(), It.IsAny<IHero>()))
                .Callback((IHero fighter1, IHero fighter2) =>
                {
                    index1 = battleServiceBuilder.AllHeroes.ToList().IndexOf(fighter1);
                    index2 = battleServiceBuilder.AllHeroes.ToList().IndexOf(fighter2);

                    Assert.That(index1, Is.GreaterThanOrEqualTo(0),
                        "You must use a hero from the repository to create the battle using the factory.");

                    Assert.That(index2, Is.GreaterThanOrEqualTo(0),
                        "You must use a hero from the repository to create the battle using the factory.");

                    Assert.That(index1, Is.Not.EqualTo(index2), "The same hero cannot be picked twice in one battle.");

                    createdBattle = new Mock<IBattle>().Object;
                })
                .Returns((IHero fighter1, IHero fighter2) => createdBattle);
            IBattleService service = battleServiceBuilder.Build();

            //Act
            IBattle battle = service.SetupRandomBattle();

            //Assert
            Assert.That(battle, Is.Not.Null, "The returned battle is null.");
            battleServiceBuilder.HeroRepositoryMock.Verify(repo => repo.GetAll(), Times.Once,
                "The repository should be used to retrieve all heroes.");
            battleServiceBuilder.BattleFactoryMock.Verify(
                factory => factory.CreateNewBattle(It.IsAny<IHero>(), It.IsAny<IHero>()), Times.Once,
                "The CreateNewBattle method of the factory has not been called.");
            Assert.That(battle, Is.SameAs(createdBattle),
                "The battle created by the factory is not the instance that is returned.");

            return (index1, index2);
        }
    }
}