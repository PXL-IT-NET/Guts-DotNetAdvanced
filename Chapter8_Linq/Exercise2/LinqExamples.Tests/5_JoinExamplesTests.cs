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
            var boys = new List<Person>
            {
                new Person{Firstname = "John", FavoriteAnimal = "Cat"},
                new Person{Firstname = "Jules", FavoriteAnimal = "Dog"},
                new Person{Firstname = "Jeffrey", FavoriteAnimal = "Dog"},
                new Person{Firstname = "Jay", FavoriteAnimal = "Horse"},
            };

            var girls = new List<Person>
            {
                new Person{Firstname = "Jane", FavoriteAnimal = "Cat"},
                new Person{Firstname = "Jennifer", FavoriteAnimal = "Turtle"},
                new Person{Firstname = "Joan", FavoriteAnimal = "Dog"},
                new Person{Firstname = "Jill", FavoriteAnimal = "Cat"},
            };

           var expected = new List<string>
           {
               "John and Jane", "John and Jill", "Jules and Joan", "Jeffrey and Joan"
           };

            //Act
            var couples = _examples.FindCouplesByFavoriteAnimalUsingJoin(boys, girls);

            //Assert
            Assert.That(couples, Is.EquivalentTo(expected));

        }
    }
}
