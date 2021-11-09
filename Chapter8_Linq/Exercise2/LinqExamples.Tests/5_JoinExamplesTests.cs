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
        public void PersonsCanBeGroupedByFavoriteAnimalUsingGroupBy()
        {
            //Arrange
            var group1 = new List<Person>
            {
                new Person{Firstname = "John", Age = 20},
                new Person{Firstname = "Jules", Age = 30},
                new Person{Firstname = "Jeffrey", Age = 30},
                new Person{Firstname = "Jay", Age = 40},
            };

            var group2 = new List<Person>
            {
                new Person{Firstname = "Jane", Age = 20},
                new Person{Firstname = "Jennifer", Age = 50},
                new Person{Firstname = "Joan", Age = 30},
                new Person{Firstname = "Jill", Age = 20},
            };

            var expected = new List<string>
            {
                "John and Jane", "John and Jill", "Jules and Joan", "Jeffrey and Joan"
            };

            //Act
            var duos = _examples.Merge2GroupsIntoDuosByAgeUsingJoin(group1, group2);

            //Assert
            Assert.That(duos, Is.EquivalentTo(expected));

        }
    }
}