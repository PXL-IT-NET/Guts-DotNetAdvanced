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
        public void ListOfNumbersCanBeSortedInDescendingOrderUsingOrderBy()
        {
            //Arrange
            int[] numbers = { 18, 5, 15, 25, 7, 29 };
            int[] expected = { 29, 25, 18, 15, 7, 5 };

            //Act
            var actual = _examples.SortNumbersDescending(numbers);

            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void PersonsCanBeSortedOnDescendingAgeAndThenOnDescendingNameUsingOrderBy()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{Name = "Joe", Age = 20},
                new Person{Name = "John", Age = 20},
                new Person{Name = "Jane", Age = 54},
                new Person{Name = "Jules", Age = 17},
                new Person{Name = "Jeffry", Age = 20}
            };

            var expected = new List<Person>
            {
                new Person{Name = "Jane", Age = 54},
                new Person{Name = "John", Age = 20},
                new Person{Name = "Joe", Age = 20},
                new Person{Name = "Jeffry", Age = 20},
                new Person{Name = "Jules", Age = 17}
            };

            //Act
            var actual = _examples.SortPersonsOnDescendingAgeAndThenOnDescendingName(persons);

            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
