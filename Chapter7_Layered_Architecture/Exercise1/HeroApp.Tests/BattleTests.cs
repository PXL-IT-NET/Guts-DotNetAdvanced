using System;
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
        @"HeroApp.Domain\Battle.cs;HeroApp.Domain\Contracts\IBattle.cs;HeroApp.Domain\Contracts\IBattleFactory.cs")]
    public class BattleTests
    {
        private Type _battleType;
        private Type _factoryType;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _battleType = typeof(Battle);
            _factoryType = _battleType.GetNestedType("Factory", BindingFlags.Public | BindingFlags.NonPublic);
        }

        [MonitoredTest("IBattle - Should not have changed interface")]
        public void ShouldNotHaveChangedIBattle()
        {
            var filePath = @"HeroApp.Domain\Contracts\IBattle.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("46-72-6A-7B-01-1E-65-EC-ED-0A-29-80-70-BE-7A-D6"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("IBattleFactory - Should not have changed interface")]
        public void ShouldNotHaveChangedIBattleFactory()
        {
            var filePath = @"HeroApp.Domain\Contracts\IBattleFactory.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("39-62-A2-0E-47-E1-FE-3C-67-32-EA-88-AA-CB-2D-A7"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Battle - Should implement IBattle")]
        public void ShouldImplementIBattle()
        {
            Assert.That(typeof(IBattle).IsAssignableFrom(_battleType), Is.True);
        }

        [MonitoredTest("Battle - Should only be visible to the domain layer")]
        public void ShouldOnlyBeVisibleToTheDomainLayer()
        {
            Assert.That(_battleType.IsNotPublic,
                "Only IBattle should be visible to the other layers. The Battle class itself can be encapsulated in the domain layer.");
        }

        [MonitoredTest("Battle - Should have a private constructor that accepts 2 fighters")]
        public void ShouldHaveAPrivateConstructorThatAccepts2Fighters()
        {
            Assert.That(_battleType.GetConstructors(BindingFlags.Instance | BindingFlags.Public), Is.Empty,
                "A public constructor is found (maybe an implicit parameterless constructor?).");

            ConstructorInfo constructor = _battleType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
            Assert.That(constructor, Is.Not.Null, "Cannot find a non-public constructor.");
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2), "The constructor should have 2 parameters.");
            Assert.That(parameters.All(p => p.ParameterType == typeof(IHero)), Is.True, "The constructor parameters should be of type IHero.");
            Assert.That(constructor.IsPrivate, Is.True,
                "The constructor should be private so it can only be accessed by the (nested) 'Factory' class.");
        }

        [MonitoredTest("Battle - Should not have setters for its properties.")]
        public void ShouldNotHaveSettersForItsProperties()
        {
            AssertHasNoSetter(nameof(IBattle.Fighter1));
            AssertHasNoSetter(nameof(IBattle.Fighter2));
            AssertHasNoSetter(nameof(IBattle.IsOver));
        }

        [MonitoredTest("Battle.Factory - Should be nested inside the Battle class")]
        public void Factory_ShouldBeNestedInsideTheBattleClass()
        {
            AssertHasNestedFactory();
        }

        [MonitoredTest("Battle.Factory - CreateNewBattle - Should create a valid battle")]
        public void Factory_CreateNewBattle_ShouldCreateAValidBattle()
        {
            //Arrange
            IBattleFactory factory = CreateFactoryInstance();
            IHero fighter1 = new HeroMockBuilder().BuildObject();
            IHero fighter2 = new HeroMockBuilder().BuildObject();

            //Act
            IBattle battle = factory.CreateNewBattle(fighter1, fighter2);

            //Assert
            Assert.That(battle, Is.Not.Null, "CreateNewBattle should not return null.");
            Assert.That(battle.Fighter1, Is.SameAs(fighter1), "CreateNewBattle does not set fighter1 correctly.");
            Assert.That(battle.Fighter2, Is.SameAs(fighter2), "CreateNewBattle does not set fighter2 correctly.");
        }

        [MonitoredTest("Battle.Factory - CreateNewBattle - Should throw ArgumentNullException for invalid input")]
        public void Factory_CreateNewBattle_ShouldThrowArgumentNullExceptionForInvalidInput()
        {
            //Arrange
            IBattleFactory factory = CreateFactoryInstance();
            IHero fighter = new HeroMockBuilder().BuildObject();

            //Act + Assert
            Assert.That(() => factory.CreateNewBattle(null, null), Throws.InstanceOf<ArgumentNullException>(),
                "Null fighters are not allowed");
            Assert.That(() => factory.CreateNewBattle(fighter, null), Throws.InstanceOf<ArgumentNullException>(),
                "Null fighters are not allowed");
            Assert.That(() => factory.CreateNewBattle(null, fighter), Throws.InstanceOf<ArgumentNullException>(),
                "Null fighters are not allowed");
        }

        [MonitoredTest("Battle - FightRound - Should let the 2 fighters attack")]
        public void FightRound_ShouldLetThe2FightersAttack()
        {
            //Arrange
            Mock<IHero> fighter1Mock = new HeroMockBuilder().WithHealth(100).Build();
            Mock<IHero> fighter2Mock = new HeroMockBuilder().WithHealth(100).Build();
            IBattle battle = new BattleBuilder(fighter1Mock.Object, fighter2Mock.Object).Build();

            //Act
            battle.FightRound();

            //Assert
            fighter1Mock.Verify(f => f.Attack(fighter2Mock.Object), Times.Once, "Fighter 1 did not attack fighter 2.");
            fighter2Mock.Verify(f => f.Attack(fighter1Mock.Object), Times.Once, "Fighter 2 did not attack fighter 1.");
        }

        [MonitoredTest("Battle - FightRound - Fighter 1 has no health - Should only let fighter 2 attack")]
        public void FightRound_Fighter1HasNoHealth_ShouldOnlyLetFighter2Attack()
        {
            //Arrange
            Mock<IHero> fighter1Mock = new HeroMockBuilder().WithHealth(0).Build();
            Mock<IHero> fighter2Mock = new HeroMockBuilder().WithHealth(100).Build();
            IBattle battle = new BattleBuilder(fighter1Mock.Object, fighter2Mock.Object).Build();

            //Act
            battle.FightRound();

            //Assert
            fighter1Mock.Verify(f => f.Attack(It.IsAny<IHero>()), Times.Never, "Fighter 1 should not attack. He has no health.");
            fighter2Mock.Verify(f => f.Attack(fighter1Mock.Object), Times.Once, "Fighter 2 did not attack fighter 1.");
        }

        [MonitoredTest("Battle - FightRound - Fighter 2 has no health - Should only let fighter 1 attack")]
        public void FightRound_Fighter2HasNoHealth_ShouldOnlyLetFighter1Attack()
        {
            //Arrange
            Mock<IHero> fighter1Mock = new HeroMockBuilder().WithHealth(100).Build();
            Mock<IHero> fighter2Mock = new HeroMockBuilder().WithHealth(0).Build();
            IBattle battle = new BattleBuilder(fighter1Mock.Object, fighter2Mock.Object).Build();

            //Act
            battle.FightRound();

            //Assert
            fighter1Mock.Verify(f => f.Attack(fighter2Mock.Object), Times.Once, "Fighter 1 did not attack fighter 2.");
            fighter2Mock.Verify(f => f.Attack(It.IsAny<IHero>()), Times.Never, "Fighter 2 should not attack. He has no health.");
        }

        [MonitoredTest("Battle - IsOver - Both fighters have health - Should return false")]
        public void IsOver_BothFightersHaveHealth_ShouldReturnFalse()
        {
            //Arrange
            IHero fighter1 = new HeroMockBuilder().WithHealth(20).BuildObject(); 
            IHero fighter2 = new HeroMockBuilder().WithHealth(10).BuildObject();
            IBattle battle = new BattleBuilder(fighter1, fighter2).Build();

            //Act + Assert
            Assert.That(battle.IsOver, Is.False);
        }

        [MonitoredTest("Battle - IsOver - A fighter has no health - Should return true")]
        public void IsOver_AFighterHasNoHealth_ShouldReturnTrue()
        {
            //Arrange
            IHero fighter1 = new HeroMockBuilder().WithHealth(-5).BuildObject();
            IHero fighter2 = new HeroMockBuilder().WithHealth(10).BuildObject();
            IBattle battle = new BattleBuilder(fighter1, fighter2).Build();

            //Act + Assert
            Assert.That(battle.IsOver, Is.True);
        }

        private void AssertHasNestedFactory()
        {
            Assert.That(_factoryType, Is.Not.Null, "Could not find class named 'Factory' nested in the 'Battle' class.");
            Assert.That(_factoryType.IsNestedAssembly, Is.True,
                "The nested 'Factory' class should not be visible for other assemblies (layers).");
            Assert.That(typeof(IBattleFactory).IsAssignableFrom(_factoryType), Is.True,
                "The nested 'Factory' class should implement the 'IBattleFactory' interface.");
            ConstructorInfo constructor = _factoryType.GetConstructors().FirstOrDefault();
            Assert.That(constructor, Is.Not.Null,
                "The nested 'Factory' class should have an (implicit) constructor.");
            Assert.That(constructor.GetParameters().Length, Is.Zero,
                "The nested 'Factory' class should have an (implicit) parameterless constructor.");

        }

        private IBattleFactory CreateFactoryInstance()
        {
            AssertHasNestedFactory();
            IBattleFactory factory = Activator.CreateInstance(_factoryType) as IBattleFactory;
            Assert.That(factory, Is.Not.Null,
                "Could not create an instance of the 'Factory' class and cast it to 'IBattleFactory'.");
            return factory;
        }

        private void AssertHasNoSetter(string propertyName)
        {
            PropertyInfo property = _battleType.GetProperty(propertyName);
            Assert.That(property, Is.Not.Null, $"No property '{propertyName}' was found");
            Assert.That(property.SetMethod, Is.Null, $"A setter is found for the '{propertyName}' property");
        }
    }
}