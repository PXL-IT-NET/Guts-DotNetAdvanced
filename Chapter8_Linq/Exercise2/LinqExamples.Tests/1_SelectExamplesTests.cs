using System;
using System.Collections.Generic;
using LinqExamples.Models;
using NUnit.Framework;

namespace LinqExamples.Tests
{
    [TestFixture]
    public class SelectExamplesTests
    {
        private SelectExamples _examples;

        [SetUp]
        public void Setup()
        {
            _examples = new SelectExamples();
        }

        [Test]
        public void NumbersCanBeProjectedToTheirSquares()
        {
            //Arrange
            var numbers = new List<int> { 1, 2, 3, 5 };
            var expected = new List<int> { 1, 4, 9, 25 };

            //Act
            var actual = _examples.ConvertNumbersToSquaredNumbers(numbers);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void PersonsCanBeProjectedToPersonSummaries()
        {
            //Arrange
            var persons = new List<Person>();
            var expected = new List<PersonSummary>();

            Guid id = Guid.NewGuid();
            persons.Add(new Person { Id = id, Age = 11, Firstname = "Bart", Lastname = "Simpson", FavoriteAnimal = "Cat" });
            expected.Add(new PersonSummary { Id = id, FullName = "Bart Simpson", IsChild = true });

            id = Guid.NewGuid();
            persons.Add(new Person { Id = id, Age = 18, Firstname = "John", Lastname = "Doe", FavoriteAnimal = "Dog" });
            expected.Add(new PersonSummary { Id = id, FullName = "John Doe", IsChild = false });

            id = Guid.NewGuid();
            persons.Add(new Person { Id = id, Age = 17, Firstname = "Jane", Lastname = "Doe", FavoriteAnimal = "Cat" });
            expected.Add(new PersonSummary { Id = id, FullName = "Jane Doe", IsChild = true });

            //Act
            var actual = _examples.ConvertPersonsToPersonSummaries(persons);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}