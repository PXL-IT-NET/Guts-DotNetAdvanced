using System;
using System.Collections.Generic;
using System.Reflection;
using HeroApp.AppLogic;
using HeroApp.AppLogic.Contracts;
using HeroApp.Domain.Contracts;
using Moq;
using NUnit.Framework;

namespace HeroApp.Tests
{
    internal class BattleServiceBuilder
    {
        private BattleService _battleService;
        private readonly List<IHero> _allHeroes;

        public Mock<IHeroRepository> HeroRepositoryMock { get; }
        public Mock<IBattleFactory> BattleFactoryMock { get; }
        public IReadOnlyList<IHero> AllHeroes => _allHeroes;

        public BattleServiceBuilder()
        {
            _allHeroes = new List<IHero>();

            HeroRepositoryMock = new Mock<IHeroRepository>();
            HeroRepositoryMock.Setup(repo => repo.GetAll()).Returns(AllHeroes);

            BattleFactoryMock = new Mock<IBattleFactory>();
            BattleFactoryMock.Setup(factory => factory.CreateNewBattle(It.IsAny<IHero>(), It.IsAny<IHero>()))
                .Returns((IHero fighter1, IHero fighter2) => new Mock<IBattle>().Object);
        }

        public BattleServiceBuilder WithHeroes(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                _allHeroes.Add(new HeroMockBuilder().BuildObject());
            }
            return this;
        }

        private void ConstructBattleService(IHeroRepository heroRepository, IBattleFactory battleFactory)
        {
            try
            {
                try
                {
                    _battleService = Activator.CreateInstance(typeof(BattleService),
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                        new object[] { battleFactory, heroRepository },
                        null) as BattleService;
                }
                catch (Exception)
                {
                    _battleService = Activator.CreateInstance(typeof(BattleService),
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                        new object[] { heroRepository, battleFactory },
                        null) as BattleService;
                }
            }
            catch (Exception)
            {
                _battleService = null;
            }
            Assert.That(_battleService, Is.Not.Null, "Failed to instantiate a BattleService.");
        }

        public virtual IBattleService Build()
        {
            ConstructBattleService(HeroRepositoryMock.Object, BattleFactoryMock.Object);
            return _battleService as IBattleService;
        }
    }
}