using System;
using System.Reflection;
using HeroApp.Domain;
using HeroApp.Domain.Contracts;
using NUnit.Framework;

namespace HeroApp.Tests
{
    internal class BattleBuilder
    {
        private Battle _battle;

        public BattleBuilder(IHero fighter1, IHero fighter2)
        {
            ConstructBattle(fighter1, fighter2);
        }

        private void ConstructBattle(params object[] constructorParameters)
        {
            try
            {
                _battle = Activator.CreateInstance(typeof(Battle),
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                    constructorParameters,
                    null) as Battle;
            }
            catch (Exception)
            {
                _battle = null;
            }
            Assert.That(_battle, Is.Not.Null, "Failed to instantiate a Battle.");
        }

        public virtual IBattle Build()
        {
            return _battle as IBattle;
        }
    }
}