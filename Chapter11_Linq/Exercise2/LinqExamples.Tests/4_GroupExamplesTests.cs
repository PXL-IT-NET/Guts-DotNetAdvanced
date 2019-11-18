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
        public void NumbersSmallerThanOrEqualTo100AndBiggerNumbersCanBeGroupedUsingGroupBy()
        {
            //Arrange
            int[] numbers = { 5, 700, 15, 108, 25, 28, 100 };
            int[] expectedSmallNumbers = { 5, 15, 25, 28, 100};
            int[] expectedBigNumbers = { 700, 108 };

            //Act
            var results = _examples.GroupSmallAndBigNumbers(numbers);

            //Assert
            Assert.That(results.Count, Is.EqualTo(2));
            var smallNumbers = results[0];
            Assert.That(smallNumbers, Is.EquivalentTo(expectedSmallNumbers));
            var bigNumbers = results[1];
            Assert.That(bigNumbers, Is.EquivalentTo(expectedBigNumbers));
        }

        [Test]
        public void PersonsCanBeGroupedByAgeUsingGroupBy()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{Firstname = "John", Age = 20},
                new Person{Firstname = "Jane", Age = 30},
                new Person{Firstname = "Jules", Age = 30},
                new Person{Firstname = "Jeffry", Age = 20},
                new Person{Firstname = "Joe", Age = 30}
            };

            //Act
            var personAgeGroups = _examples.GroupPersonsByAge(persons);

            //Assert
            Assert.That(personAgeGroups.Count,Is.EqualTo(2));

            var twentiers = personAgeGroups.FirstOrDefault(group => group.Age == 20);
            Assert.That(twentiers, Is.Not.Null);
            Assert.That(twentiers.Persons.Count, Is.EqualTo(2));
            Assert.That(twentiers.Persons, Has.All.Matches((Person p) => p.Age == 20));

            var thirtiers = personAgeGroups.FirstOrDefault(group => group.Age == 30);
            Assert.That(thirtiers, Is.Not.Null);
            Assert.That(thirtiers.Persons.Count, Is.EqualTo(3));
            Assert.That(thirtiers.Persons, Has.All.Matches((Person p) => p.Age == 30));
        }
    }
}
