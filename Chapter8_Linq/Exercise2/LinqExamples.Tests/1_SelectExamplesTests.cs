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
        public void WordsCanBeProjectsToTheirUpperCaseVersion()
        {
            //Arrange
            var words = new List<string> { "aap", "Banaan", "CHOCO", "dEuRmAt" };
            var expected = new List<string> { "AAP", "BANAAN", "CHOCO", "DEURMAT" };

            //Act
            var actual = _examples.ConvertWordsToUpper(words);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void PersonsCanBeProjectedToPersonSummaries()
        {
            //Arrange
            var persons = new List<Person>
            {
                new Person{ Id = 1, Age = 11, Firstname = "Bart", Lastname = "Simpson", FavoriteAnimal = "Cat"},
                new Person{ Id = 2, Age = 18, Firstname = "John", Lastname = "Doe", FavoriteAnimal = "Dog"},
                new Person{ Id = 3, Age = 38, Firstname = "Jane", Lastname = "Doe", FavoriteAnimal = "Cat"}
            };
            var expected = new List<PersonSummary>
            {
                new PersonSummary{ FullName = "Bart Simpson", IsAdult = false},
                new PersonSummary{ FullName = "John Doe", IsAdult = true},
                new PersonSummary{ FullName = "Jane Doe", IsAdult = true}
            };

            //Act
            var actual = _examples.ConvertPersonsToPersonSummaries(persons);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
