using System.Collections.Generic;
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
        public void NumbersDivisibleByFiveCanBeFilteredOutUsingWhere()
        {
            //Arrange
            int[] numbers = { 5, 7, 15, 18, 25, 29 };
            int[] expected = { 5, 15, 25 };

            //Act
            var actual = _examples.FilterOutNumbersDivisibleByFive(numbers);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void PersonsThatAreEighteenOrOlderCanBeFilteredOutUsingWhere()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{Name = "John", Age = 11},
                new Person{Name = "Jane", Age = 54},
                new Person{Name = "Jules", Age = 17},
                new Person{Name = "Jeffry", Age = 20},
                new Person{Name = "Joe", Age = 15}
            };

            //Act
            var adults = _examples.FilterOutPersonsThatAreEighteenOrOlder(persons);

            //Assert
            Assert.That(adults, Has.All.Matches((Person p) => p.Age >= 18));
        }
    }
}
