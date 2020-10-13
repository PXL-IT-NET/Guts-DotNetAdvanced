using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using HeroApp.Domain;
using HeroApp.Domain.Contracts;
using Moq;
using NUnit.Framework;

namespace HeroApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise01",
        @"HeroApp.Domain\Hero.cs;HeroApp.Domain\Contracts\IHero.cs;HeroApp.Domain\Contracts\IHeroFactory.cs")]
    public class HeroTests
    {
        private Type _heroType;
        private Type _factoryType;
        private static Random Random = new Random();

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _heroType = typeof(Hero);
            _factoryType = _heroType.GetNestedType("Factory", BindingFlags.Public | BindingFlags.NonPublic);
        }

        [MonitoredTest("Hero - Should implement IHero")]
        public void ShouldImplementIHero()
        {
            Assert.That(typeof(IHero).IsAssignableFrom(_heroType), Is.True);
        }

        [MonitoredTest("IHero - Should not have changed interface")]
        public void ShouldNotHaveChangedIHero()
        {
            var filePath = @"HeroApp.Domain\Contracts\IHero.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("91-2E-42-6A-FC-9F-3C-F4-B4-AA-A5-E4-18-C0-FB-0E"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("IHeroFactory - Should not have changed interface")]
        public void ShouldNotHaveChangedIHeroFactory()
        {
            var filePath = @"HeroApp.Domain\Contracts\IHeroFactory.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("68-80-BB-12-57-60-07-9D-AF-1C-F9-34-F9-F1-98-D5"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Hero - Should only be visible to the domain layer")]
        public void ShouldOnlyBeVisibleToTheDomainLayer()
        {
            Assert.That(_heroType.IsNotPublic,
                "Only IHero should be visible to the other layers. The Hero class itself can be encapsulated in the domain layer.");
        }

        [MonitoredTest("Hero - Should not have public constructors")]
        public void ShouldNotHavePublicConstructors()
        {
            Assert.That(_heroType.GetConstructors(BindingFlags.Instance | BindingFlags.Public), Is.Empty,
                "A public constructor is found (maybe an implicit parameterless constructor?).");

            ConstructorInfo constructor = _heroType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
            Assert.That(constructor, Is.Not.Null, "Cannot find a non-public constructor.");
            Assert.That(constructor.GetParameters().Length, Is.Zero, "The constructor should not have parameters.");
            Assert.That(constructor.IsPrivate, Is.True,
                "The constructor should be private so it can only be accessed by the (nested) 'Factory' class.");
        }

        [MonitoredTest("Hero - Should have private setters for its properties.")]
        public void ShouldHavePrivateSettersForItsProperties()
        {
            AssertHasPrivateSetter(nameof(IHero.Name));
            AssertHasPrivateSetter(nameof(IHero.Strength));
            AssertHasPrivateSetter(nameof(IHero.SuperModeLikeliness));
            AssertHasPrivateSetter(nameof(IHero.Health));
        }

        [MonitoredTest("Hero.Factory - Should be nested inside the Hero class")]
        public void Factory_ShouldBeNestedInsideTheHeroClass()
        {
            AssertHasNestedFactory();
        }

        [MonitoredTest("Hero.Factory - CreateNewHero - Should create a valid hero")]
        public void Factory_CreateNewHero_ShouldCreateAValidHero()
        {
            //Arrange
            IHeroFactory factory = CreateFactoryInstance();

            //Act
            AssertCreateValidHero(factory, "Joe", 50, 0.5f);
            AssertCreateValidHero(factory, "John", 1, 0.0f);
            AssertCreateValidHero(factory, "Jane", 100, 1.0f);
        }

        [MonitoredTest("Hero.Factory - CreateNewHero - Should throw ArgumentException for invalid input")]
        public void Factory_CreateNewHero_ShouldThrowArgumentExceptionForInvalidInput()
        {
            //Arrange
            IHeroFactory factory = CreateFactoryInstance();

            //Act
            Assert.That(() => factory.CreateNewHero("", 50, 0.5f), Throws.InstanceOf<ArgumentException>(),
                "An empty name should not be allowed");
            Assert.That(() => factory.CreateNewHero(null, 50, 0.5f), Throws.InstanceOf<ArgumentException>(),
                "An empty name should not be allowed");

            Assert.That(() => factory.CreateNewHero("John", 0, 0.5f), Throws.InstanceOf<ArgumentException>(),
                "Strength of zero or less should not be allowed");
            Assert.That(() => factory.CreateNewHero("John", -10, 0.5f), Throws.InstanceOf<ArgumentException>(),
                "Strength of zero or less should not be allowed");
            Assert.That(() => factory.CreateNewHero("John", 101, 0.5f), Throws.InstanceOf<ArgumentException>(),
                "Strength bigger than 100 should not be allowed");
            Assert.That(() => factory.CreateNewHero("John", 120, 0.5f), Throws.InstanceOf<ArgumentException>(),
                "Strength bigger than 100 should not be allowed");

            Assert.That(() => factory.CreateNewHero("John", 50, -1.0f), Throws.InstanceOf<ArgumentException>(),
                "A negative supermode likeliness should not be allowed");
            Assert.That(() => factory.CreateNewHero("John", 50, -10.0f), Throws.InstanceOf<ArgumentException>(),
                "A negative supermode likeliness should not be allowed");
            Assert.That(() => factory.CreateNewHero("John", 50, 1.1f), Throws.InstanceOf<ArgumentException>(),
                "A supermode likeliness bigger than 1.0f should not be allowed");
            Assert.That(() => factory.CreateNewHero("John", 50, 10.0f), Throws.InstanceOf<ArgumentException>(),
                "A supermode likeliness bigger than 1.0f should not be allowed");

        }

        [MonitoredTest("Hero - Attack - Should throw InvalidOperationException when the hero has no health")]
        public void Attack_ShouldThrowInvalidOperationExceptionWhenTheHeroHasNoHealth()
        {
            //Arrange
            IHero hero = new HeroBuilder().WithHealth(0).Build();
            IHero opponent = new HeroBuilder().Build();

            //Act + Assert
            Assert.That(() => hero.Attack(opponent), Throws.InstanceOf<InvalidOperationException>());
        }

        [MonitoredTest("Hero - Attack - NoSuperMode - Should attack opponent with normal strength")]
        public void Attack_NoSuperMode_ShouldAttackOpponentWithNormalStrength()
        {
            //Arrange
            IHero hero = new HeroBuilder().WithSuperModeLikeliness(0).Build();
            Mock<IHero> opponentMock = new HeroMockBuilder().Build();

            //Act
           hero.Attack(opponentMock.Object);

           //Assert
           opponentMock.Verify(opponent => opponent.DefendAgainstAttack(hero.Strength), Times.Once,
               $"Should call 'DefendAgainstAttack' of opponent with strength {hero.Strength}.");
        }

        [MonitoredTest("Hero - Attack - 100% SuperModeLikeliness - Should attack opponent with double strength")]
        public void Attack_SuperMode_ShouldAttackOpponentWithDoubleStrength()
        {
            //Arrange
            IHero hero = new HeroBuilder().WithSuperModeLikeliness(1.0f).Build();
            Mock<IHero> opponentMock = new HeroMockBuilder().Build();

            //Act
            hero.Attack(opponentMock.Object);

            //Assert
            opponentMock.Verify(opponent => opponent.DefendAgainstAttack(hero.Strength * 2), Times.Once,
                $"Should call 'DefendAgainstAttack' of opponent with strength {hero.Strength * 2}.");
        }

        [MonitoredTest("Hero - Attack - 50% SuperModeLikeliness - Should attack opponent with normal or double strength")]
        public void Attack_50PercentSuperModeLikeliness_ShouldAttackOpponentWithNormalOrDoubleStrength()
        {
            IHero hero = new HeroBuilder().WithSuperModeLikeliness(0.5f).Build();

            int numberOfAttacks = 100;
            int numberOfNormalAttacks = 0;
            int numberOfSuperAttacks = 0;

            for (int i = 0; i < numberOfAttacks; i++)
            {
                
                Mock<IHero> opponentMock = new HeroMockBuilder().Build();
                opponentMock.Setup(opponent => opponent.DefendAgainstAttack(It.IsAny<int>()))
                    .Callback((int attackStrength) =>
                    {
                        if (attackStrength == hero.Strength)
                        {
                            numberOfNormalAttacks++;
                        }
                        else if(attackStrength == hero.Strength * 2)
                        {
                            numberOfSuperAttacks++;
                        }
                    });
                hero.Attack(opponentMock.Object);
            }

            Assert.That(numberOfNormalAttacks, Is.GreaterThan(0),
                $"Out of {numberOfAttacks} attacks, no normal attack happened. That is not random enough.");
            Assert.That(numberOfSuperAttacks, Is.GreaterThan(0),
                $"Out of {numberOfAttacks} attacks, no supermode attack happened. That is not random enough.");
            Assert.That(numberOfSuperAttacks + numberOfNormalAttacks, Is.EqualTo(numberOfAttacks),
                $"The amount of normal attacks ({numberOfNormalAttacks}) " +
                $"added with the number of supermode attacks ({numberOfSuperAttacks}) " +
                $"should be equal to the total number of attacks ({numberOfAttacks}).");
        }

        [MonitoredTest("Hero - DefendAgainstAttack - NoSuperMode - Should decrement health with attack strength")]
        public void DefendAgainstAttack_NoSuperMode_ShouldDecrementHealthWithAttackStrength()
        {
            //Arrange
            int health = Random.Next(50, 100);
            IHero hero = new HeroBuilder().WithSuperModeLikeliness(0).WithHealth(health).Build();
            int attackStrength = Random.Next(1, hero.Health);

            //Act
            hero.DefendAgainstAttack(attackStrength);

            //Assert
            Assert.That(hero.Health, Is.EqualTo(health - attackStrength));
        }

        [MonitoredTest("Hero - DefendAgainstAttack - SuperMode - Should decrement health with half attack strength")]
        public void DefendAgainstAttack_NoSuperMode_ShouldDecrementHealthWithHalfAttackStrength()
        {
            //Arrange
            int health = Random.Next(50, 100);
            IHero hero = new HeroBuilder().WithSuperModeLikeliness(1).WithHealth(health).Build();
            int attackStrength = Random.Next(1, hero.Health);

            //Act
            hero.DefendAgainstAttack(attackStrength);

            //Assert
            Assert.That(hero.Health, Is.EqualTo(health - attackStrength / 2));
        }

        [MonitoredTest("Hero - DefendAgainstAttack - 50% SuperModeLikeliness - Should decrement health with attack strength or half attack strength")]
        public void DefendAgainstAttack_50PercentSuperModeLikeliness_ShouldDecrementHealthWithAttackStrengthOrHalfAttackStrength()
        {
            int health = 100;
            IHero hero = new HeroBuilder().WithSuperModeLikeliness(0.5f).WithHealth(health).Build();
            int attackStrength = 2;
            int numberOfDefends = health / attackStrength;

            for (int i = 0; i < numberOfDefends; i++)
            {
                hero.DefendAgainstAttack(attackStrength);
            }

            int maximumHealth = health - (numberOfDefends * (attackStrength / 2));
            Assert.That(hero.Health, Is.LessThan(maximumHealth),
                $"After {numberOfDefends} defends against an attack of strength {attackStrength}, the health of the hero should be less than {maximumHealth}.");

            int minimumHealth = health - (numberOfDefends * attackStrength);
            Assert.That(hero.Health, Is.GreaterThan(minimumHealth),
                $"After {numberOfDefends} defends against an attack of strength {attackStrength}, the health of the hero should be more than {minimumHealth}.");
        }

        [MonitoredTest("Hero - Should notify changes in health")]
        public void ShouldNotifyChangesInHealth()
        {
            HeroBuilder builder = new HeroBuilder().WithHealth(100);
            IHero hero = builder.Build();

            INotifyPropertyChanged notifier = hero as INotifyPropertyChanged;
            Assert.That(notifier, Is.Not.Null,
                "Hero class should implement an interface that enables property change notifications.");

            IList<string> changedProperties = new List<string>();
            notifier.PropertyChanged += (sender, args) =>
            {
                changedProperties.Add(args.PropertyName);
            };

            //trigger Health change
            builder.WithHealth(50);

            Assert.That(changedProperties.FirstOrDefault(), Is.EqualTo(nameof(IHero.Health)), "A change in Health is not notified.");
        }

        private static void AssertCreateValidHero(IHeroFactory factory, string name, int strength, float superModeLikeliness)
        {
            IHero hero = factory.CreateNewHero(name, strength, superModeLikeliness);
            string call = $@"CreateNewHero(""{name}"", {strength}, {superModeLikeliness})";
            Assert.That(hero, Is.Not.Null, $"{call} should not return null.");
            Assert.That(hero.Name, Is.EqualTo(name), $"{call} does not set name correctly.");
            Assert.That(hero.Strength, Is.EqualTo(strength), $"{call} does not set strength correctly.");
            Assert.That(hero.SuperModeLikeliness, Is.EqualTo(superModeLikeliness),
                $"{call} does not set supermode likeliness correctly.");
            Assert.That(hero.Health, Is.EqualTo(100), "Health should be 100.");
        }

        private void AssertHasNestedFactory()
        {
            Assert.That(_factoryType, Is.Not.Null, "Could not find class named 'Factory' nested in the 'Hero' class.");
            Assert.That(_factoryType.IsNestedAssembly, Is.True,
                "The nested 'Factory' class should not be visible for other assemblies (layers).");
            Assert.That(typeof(IHeroFactory).IsAssignableFrom(_factoryType), Is.True,
                "The nested 'Factory' class should implement the 'IHeroFactory' interface.");
            ConstructorInfo constructor = _factoryType.GetConstructors().FirstOrDefault();
            Assert.That(constructor, Is.Not.Null,
                "The nested 'Factory' class should have an (implicit) constructor.");
            Assert.That(constructor.GetParameters().Length, Is.Zero,
                "The nested 'Factory' class should have an (implicit) parameterless constructor.");

        }

        private IHeroFactory CreateFactoryInstance()
        {
            AssertHasNestedFactory();
            IHeroFactory factory = Activator.CreateInstance(_factoryType) as IHeroFactory;
            Assert.That(factory, Is.Not.Null,
                "Could not create an instance of the 'Factory' class and cast it to 'IHeroFactory'.");
            return factory;
        }

        private void AssertHasPrivateSetter(string propertyName)
        {
            PropertyInfo property = _heroType.GetProperty(propertyName);
            Assert.That(property, Is.Not.Null, $"No property '{propertyName}' was found");
            Assert.That(property.SetMethod, Is.Not.Null, $"No setter found for the '{propertyName}' property");
            Assert.That(property.SetMethod.IsPrivate, Is.True,
                $"'{propertyName}' does not have a private setter. Specify the 'private' keyword right before the 'set' keyword.");
        }
    }
}
