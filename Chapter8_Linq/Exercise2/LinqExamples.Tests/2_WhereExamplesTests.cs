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
        public void NumbersOutOfRangeCanBeFilteredOutUsingWhere()
        {
            //Arrange
            int[] numbers = { 300, 5, 70, 15, 108, 25, 71 };
            int[] expected = { 70, 15, 25 };

            //Act
            var actual = _examples.FilterOutNumbersOutOfRange(numbers, 15, 70);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void PersonsThatLoveCatsCanBeFilteredOutUsingWhere()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{ Id = 1, Firstname = "A", FavoriteAnimal = "Cat"},
                new Person{ Id = 2, Firstname = "B", FavoriteAnimal = "Dog"},
                new Person{ Id = 3, Firstname = "C",  FavoriteAnimal = "cat"},
                new Person{ Id = 4, Firstname = "D",  FavoriteAnimal = "Horse"},
                new Person{ Id = 5, Firstname = "E",  FavoriteAnimal = "Cat"},
            };
            int[] expectedIds = {2, 4};

            //Act
            var nonCatLovers = _examples.FilterOutCatLovers(persons);

            //Assert
            Assert.That(nonCatLovers, Has.All.Matches((Person p) => expectedIds.Contains(p.Id)));
        }
    }
}
