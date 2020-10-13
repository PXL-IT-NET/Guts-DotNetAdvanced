using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using HeroApp.Business.Contracts;
using HeroApp.Data;
using HeroApp.Domain.Contracts;
using Moq;
using NUnit.Framework;

namespace HeroApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise01",
        @"HeroApp.Data\InMemoryHeroRepository.cs;HeroApp.Business\Contracts\IHeroRepository.cs")]
    public class InMemoryHeroRepositoryTests
    {
        private Type _inMemoryHeroRepositoryType;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _inMemoryHeroRepositoryType = typeof(InMemoryHeroRepository);
        }

        [MonitoredTest("IHeroRepository - Should not have changed interface")]
        public void ShouldNotHaveChangedIHeroRepository()
        {
            var filePath = @"HeroApp.Business\Contracts\IHeroRepository.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("50-C6-4C-E7-E8-63-59-6C-AC-30-09-7F-9B-87-1F-A2"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("InMemoryHeroRepository - Should implement IHeroRepository")]
        public void ShouldImplementIHeroRepository()
        {
            Assert.That(typeof(IHeroRepository).IsAssignableFrom(_inMemoryHeroRepositoryType), Is.True);
        }

        [MonitoredTest("InMemoryHeroRepository - Should only be visible to the data layer")]
        public void ShouldOnlyBeVisibleToTheDataLayer()
        {
            Assert.That(_inMemoryHeroRepositoryType.IsNotPublic);
        }

        [MonitoredTest("InMemoryHeroRepository - Should have a constructor that accepts a hero factory")]
        public void ShouldHaveAConstructorThatAcceptsAHeroFactory()
        {
            ConstructorInfo[] constructors = _inMemoryHeroRepositoryType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");

            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(1), "The constructor should have 1 parameter.");
            Assert.That(parameters.First().ParameterType == typeof(IHeroFactory), Is.True,
                "The constructor parameter should be of type IHeroFactory.");
        }


        [MonitoredTest("InMemoryHeroRepository - GetAll - Should return at least 3 heroes")]
        public void GetAll_ShouldReturnAtLeas3Heroes()
        {
            //Arrange
            InMemoryHeroRepositoryBuilder builder = new InMemoryHeroRepositoryBuilder();
            IHeroRepository repository = builder.Build();

            //Act
            IReadOnlyList<IHero> allHeroes = repository.GetAll();

            //Assert
            Assert.That(allHeroes, Is.Not.Null, "The returned list is null.");
            Assert.That(allHeroes.Count, Is.GreaterThanOrEqualTo(3), "At least 3 heroes must be returned.");
            builder.HeroFactoryMock.Verify(
                factory => factory.CreateNewHero(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float>()),
                Times.Exactly(allHeroes.Count), 
                $"When {allHeroes.Count} heroes are returned, " +
                $"the 'CreateNewHero' method of the factory should have been called {allHeroes.Count} times.");
        }
    }
}