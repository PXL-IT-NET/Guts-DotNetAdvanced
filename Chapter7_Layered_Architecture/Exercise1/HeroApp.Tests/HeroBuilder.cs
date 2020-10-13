using System;
using System.Reflection;
using HeroApp.Domain;
using HeroApp.Domain.Contracts;
using NUnit.Framework;

namespace HeroApp.Tests
{
    internal class HeroBuilder
    {
        private static Random Random = new Random();
        private Hero _hero;

        public HeroBuilder()
        {
            ConstructHero();
            SetProperty(nameof(IHero.Name), Guid.NewGuid().ToString());
            SetProperty(nameof(IHero.Strength), Random.Next(1, 101));
            SetProperty(nameof(IHero.SuperModeLikeliness), Convert.ToSingle(Random.NextDouble()));
            SetProperty(nameof(IHero.Health), 100);
        }

        public HeroBuilder WithHealth(int health)
        {
            SetProperty(nameof(IHero.Health), health);
            return this;
        }

        public HeroBuilder WithSuperModeLikeliness(float superModeLikeliness)
        {
            SetProperty(nameof(IHero.SuperModeLikeliness), superModeLikeliness);
            return this;
        }

        private void ConstructHero()
        {
            try
            {
                _hero = Activator.CreateInstance(typeof(Hero),
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                    null,
                    null) as Hero;
            }
            catch (Exception)
            {
                _hero = null;
            }
            Assert.That(_hero, Is.Not.Null, "Failed to instantiate a Hero.");
        }

        protected void SetProperty<TProperty>(string propertyName, TProperty value)
        {
            var propertyInfo = typeof(Hero).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"Cannot find a property: {propertyName}");
            }
            propertyInfo.SetValue(_hero, value);
        }

        public virtual IHero Build()
        {
            return _hero as IHero;
        }
    }
}