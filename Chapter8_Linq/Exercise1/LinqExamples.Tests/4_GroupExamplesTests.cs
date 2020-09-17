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
        public void EvenAndOddNumbersCanBeGroupedUsingGroupBy()
        {
            //Arrange
            int[] numbers = { 5, 7, 15, 18, 25, 28 };
            int[] expectedOddNumbers = { 5, 7, 15, 25};
            int[] expectedEvenNumbers = { 18, 28 };

            //Act
            var results = _examples.GroupEvenAndOddNumbers(numbers);

            //Assert
            Assert.That(results.Count, Is.EqualTo(2));
            var oddNumbers = results[0];
            Assert.That(oddNumbers, Is.EquivalentTo(expectedOddNumbers));
            var evenNumbers = results[1];
            Assert.That(evenNumbers, Is.EquivalentTo(expectedEvenNumbers));
        }

        [Test]
        public void PersonsCanBeGroupedByAgeUsigGroupBy()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{Name = "John", Age = 20},
                new Person{Name = "Jane", Age = 30},
                new Person{Name = "Jules", Age = 30},
                new Person{Name = "Jeffry", Age = 20},
                new Person{Name = "Joe", Age = 30}
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
