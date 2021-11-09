using System;
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
        public void ListOfWordsCanBeSortedByLengthInDescendingOrderUsingOrderBy()
        {
            //Arrange
            string[] words = { "bca", "a", "acdc", "qdqd" };
            string[] expected = { "acdc", "qdqd", "bca", "a" };

            //Act
            var actual = _examples.SortWordsByLengthDescending(words);

            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void PersonsCanBeSortedOnAscendingAgeAndThenOnFavoriteAnimalDescendingUsingOrderBy()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{ Id = Guid.NewGuid(), Firstname = "A", Age = 25, FavoriteAnimal = "Horse"},
                new Person{ Id = Guid.NewGuid(), Firstname = "B", Age = 87, FavoriteAnimal = "Cat"},
                new Person{ Id = Guid.NewGuid(), Firstname = "C", Age = 15, FavoriteAnimal = "Cat"},
                new Person{ Id = Guid.NewGuid(), Firstname = "D", Age = 25, FavoriteAnimal = "Cat"},
                new Person{ Id = Guid.NewGuid(), Firstname = "E", Age = 87, FavoriteAnimal = "Dog"},
            };
            Guid[] expectedIds = { persons[2].Id, persons[0].Id, persons[3].Id, persons[4].Id, persons[1].Id };

            //Act
            var sortedPersons = _examples.SortPersonsOnAscendingAgeAndThenOnFavoriteAnimalDescending(persons);

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
