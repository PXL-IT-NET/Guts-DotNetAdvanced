using System.Collections.Generic;
using LinqExamples.Models;
using NUnit.Framework;

namespace LinqExamples.Tests
{
    [TestFixture]
    public class OrderByExamplesTests
    {
        private OrderByExamples _examples;

        [SetUp]
        public void Setup()
        {
            _examples = new OrderByExamples();
        }

        [Test]
        public void ListOfWordsCanBeSortedInDescendingOrderUsingOrderBy()
        {
            //Arrange
            string[] words = { "bca", "def", "abc", "fed" };
            string[] expected = { "fed", "def", "bca", "abc" };

            //Act
            var actual = _examples.SortWordsDescending(words);

            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void PersonsCanBeSortedOnDescendingAgeAndThenOnFavoriteAnimalAscendingUsingOrderBy()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{ Id = 1, Firstname = "A", Age = 25, FavoriteAnimal = "Horse"},
                new Person{ Id = 2, Firstname = "B", Age = 87, FavoriteAnimal = "Dog"},
                new Person{ Id = 3, Firstname = "C", Age = 15, FavoriteAnimal = "Cat"},
                new Person{ Id = 4, Firstname = "D", Age = 25, FavoriteAnimal = "Cat"},
                new Person{ Id = 5, Firstname = "E", Age = 87, FavoriteAnimal = "Cat"},
            };

            int[] expectedIds = { 5, 2, 4, 1, 3 };

            //Act
            var sortedPersons = _examples.SortPersonsOnDescendingAgeAndThenOnFavoriteAnimalAscending(persons);

            //Assert
            Assert.That(sortedPersons, Has.Count.EqualTo(expectedIds.Length));
            for (var index = 0; index < expectedIds.Length; index++)
            {
                var expectedId = expectedIds[index];
                Assert.That(sortedPersons[index].Id, Is.EqualTo(expectedId));
            }
        }
    }
}
