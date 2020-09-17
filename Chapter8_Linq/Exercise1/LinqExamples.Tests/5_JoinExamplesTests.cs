using System.Collections.Generic;
using System.Linq;
using LinqExamples.Models;
using NUnit.Framework;

namespace LinqExamples.Tests
{
    [TestFixture]
    public class JoinExamplesTests
    {
        private JoinExamples _examples;

        [SetUp]
        public void Setup()
        {
            _examples = new JoinExamples();
        }

        [Test]
        public void IntersectionOfNumbersInTwoListsCanBeFoundUsingJoin()
        {
            //Arrange
            int[] firstList = { 2, 3, 5 };
            int[] secondList = { 3, 8, 2 };
            int[] expected = { 2, 3 };

            //Act
            var intersection = _examples.GetIntersection(firstList, secondList);

            //Assert
            Assert.That(intersection, Has.All.Matches((int n) => expected.Contains(n)));
        }

        [Test]
        public void PersonsCanBeGroupedByAgeUsigGroupBy()
        {
            //Arrange
            var boys = new List<Person>
            {
                new Person{Name = "John", Age = 12},
                new Person{Name = "Jules", Age = 20},
                new Person{Name = "Jeffry", Age = 20},
                new Person{Name = "Jay", Age = 18},
            };

            var girls = new List<Person>
            {
                new Person{Name = "Jane", Age = 16},
                new Person{Name = "Jennifer", Age = 18},
                new Person{Name = "Joan", Age = 20},
                new Person{Name = "Jill", Age = 20},
            };

            var expected = new List<string>
            {
                "Jules and Joan", "Jules and Jill", "Jeffry and Joan", "Jeffry and Jill", "Jay and Jennifer"
            };

            //Act
            var couples = _examples.FindCouplesByAgeUsingJoin(boys, girls);

            //Assert
            Assert.That(couples, Is.EquivalentTo(expected));

        }
    }
}