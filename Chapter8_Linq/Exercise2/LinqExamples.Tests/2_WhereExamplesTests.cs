using System;
using System.Collections.Generic;
using System.Linq;
using LinqExamples.Models;
using NUnit.Framework;

namespace LinqExamples.Tests
{
    [TestFixture]
    public class WhereExamplesTests
    {
        private WhereExamples _examples;

        [SetUp]
        public void Setup()
        {
            _examples = new WhereExamples();
        }

        [Test]
        public void NumbersInACertainRangeCanBeFilteredOutUsingWhere()
        {
            //Arrange
            int[] numbers = { 300, 5, 70, 15, 108, 25, 71 };
            int[] expected = { 300, 5, 108, 71 };

            //Act
            var actual = _examples.FilterOutNumbersInRange(numbers, 15, 70);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void PersonsThatLoveCatsOrDogsCanBeFilteredOutUsingWhere()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{ Id = Guid.NewGuid(), Firstname = "A", FavoriteAnimal = "Cat"},
                new Person{ Id = Guid.NewGuid(), Firstname = "B", FavoriteAnimal = "Dog"},
                new Person{ Id = Guid.NewGuid(), Firstname = "C",  FavoriteAnimal = "Pig"},
                new Person{ Id = Guid.NewGuid(), Firstname = "D",  FavoriteAnimal = "Horse"},
                new Person{ Id = Guid.NewGuid(), Firstname = "E",  FavoriteAnimal = "cat"},
            };
            Guid[] expectedIds = { persons[2].Id, persons[3].Id };

            //Act
            var nonCatOrDogLovers = _examples.FilterOutCatLoversAndDogLovers(persons);

            //Assert
            Assert.That(nonCatOrDogLovers, Has.All.Matches((Person p) => expectedIds.Contains(p.Id)));
        }
    }
}