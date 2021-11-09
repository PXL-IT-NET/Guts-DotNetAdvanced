using System.Collections.Generic;
using System.Linq;
using LinqExamples.Models;
using NUnit.Framework;

namespace LinqExamples.Tests
{
    [TestFixture]
    public class GroupExamplesTests
    {
        private GroupExamples _examples;

        [SetUp]
        public void Setup()
        {
            _examples = new GroupExamples();
        }

        [Test]
        public void NegativeNumbersAndPositiveNumbersCanBeGroupedUsingGroupBy()
        {
            //Arrange
            int[] numbers = { 5, -700, -15, 108, 0, 25, -28, 100 };
            int[] expectedNegativeNumbers = { -700, -15, -28 };
            int[] expectedPositiveNumbers = { 5, 108, 0, 25, 100 };

            //Act
            var results = _examples.GroupNegativeAndPositiveNumbers(numbers);

            //Assert
            Assert.That(results.Count, Is.EqualTo(2));
            IGrouping<bool, int> negativeNumbers = results.FirstOrDefault(group => group.Any(n => n < 0));
            Assert.That(negativeNumbers, Is.Not.Null);
            Assert.That(negativeNumbers, Is.EquivalentTo(expectedNegativeNumbers));

            var positiveNumbers = results.FirstOrDefault(group => group.Any(n => n >= 0));
            Assert.That(positiveNumbers, Is.Not.Null);
            Assert.That(positiveNumbers, Is.EquivalentTo(expectedPositiveNumbers));
        }

        [Test]
        public void PersonsCanBeGroupedByFavoriteAnimalUsingGroupBy()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{Firstname = "John", FavoriteAnimal = "Cat"},
                new Person{Firstname = "Jane", FavoriteAnimal = "Dog"},
                new Person{Firstname = "Jules", FavoriteAnimal = "Pig"},
                new Person{Firstname = "Jeffry", FavoriteAnimal = "Dog"},
                new Person{Firstname = "Joe", FavoriteAnimal = "Cat"}
            };

            //Act
            var animalLoverCollections = _examples.GroupPersonsByFavoriteAnimal(persons);

            //Assert
            Assert.That(animalLoverCollections.Count, Is.EqualTo(3));

            AssertHasAnimalLoverCollection(animalLoverCollections, "Cat", 2);
            AssertHasAnimalLoverCollection(animalLoverCollections, "Dog", 2);
            AssertHasAnimalLoverCollection(animalLoverCollections, "Pig", 1);
        }

        private void AssertHasAnimalLoverCollection(IList<AnimalLoverCollection> animalLoverCollections, string lovedAnimal, int expectedPersonCount)
        {
            var animalLovers = animalLoverCollections.FirstOrDefault(group => @group.Animal == lovedAnimal);
            Assert.That(animalLovers, Is.Not.Null);
            Assert.That(animalLovers.Persons.Count, Is.EqualTo(expectedPersonCount));
            Assert.That(animalLovers.Persons, Has.All.Matches((Person p) => p.FavoriteAnimal == lovedAnimal));
        }
    }
}
