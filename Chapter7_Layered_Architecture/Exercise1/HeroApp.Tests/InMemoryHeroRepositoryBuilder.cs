using System;
using System.Reflection;
using HeroApp.Business.Contracts;
using HeroApp.Data;
using HeroApp.Domain.Contracts;
using Moq;
using NUnit.Framework;

namespace HeroApp.Tests
{
    internal class InMemoryHeroRepositoryBuilder
    {
        private InMemoryHeroRepository _heroRepository;

        public Mock<IHeroFactory> HeroFactoryMock { get; }

        public InMemoryHeroRepositoryBuilder()
        {
            HeroFactoryMock = new Mock<IHeroFactory>();
            HeroFactoryMock.Setup(factory => factory.CreateNewHero(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float>()))
                .Returns(new HeroMockBuilder().BuildObject());
            ConstructRepository(HeroFactoryMock.Object);
        }

        private void ConstructRepository(IHeroFactory heroFactory)
        {
            try
            {

                _heroRepository = Activator.CreateInstance(typeof(InMemoryHeroRepository),
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                    new object[] { heroFactory },
                    null) as InMemoryHeroRepository;
            }
            catch (Exception)
            {
                _heroRepository = null;
            }
            Assert.That(_heroRepository, Is.Not.Null, "Failed to instantiate an InMemoryHeroRepository.");
        }

        public virtual IHeroRepository Build()
        {
            return _heroRepository as IHeroRepository;
        }
    }
}