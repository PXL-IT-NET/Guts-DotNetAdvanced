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
        public void DoublesCanBeRoundedToIntegersUsingProjection()
        {
            //Arrange
            var doubles = new List<double> { 1.1, 1.6, 25.0, 50.37 };
            var expected = new List<double> { 1.0, 2.0, 25.0, 50.0 };

            //Act
            var actual = _examples.RoundDoublesUsingProjection(doubles);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void AnglesCanBeProjectedToObjectsWithAngleCosinusAndSinus()
        {
            //Arrange
            var angles = new List<double> { 10d, 45d, 90d };
            var expected = new List<AngleInfo>
            {
                new AngleInfo {Angle = 10, Cosinus = Math.Cos(10 * Math.PI / 180), Sinus = Math.Sin(10 * Math.PI / 180)},
                new AngleInfo {Angle = 45, Cosinus = Math.Cos(45 * Math.PI / 180), Sinus = Math.Sin(45 * Math.PI / 180)},
                new AngleInfo {Angle = 90, Cosinus = Math.Cos(90 * Math.PI / 180), Sinus = Math.Sin(90 * Math.PI / 180)}
            };

            //Act
            var actual = _examples.ConvertAnglesToAngleInfos(angles);

            //Assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
