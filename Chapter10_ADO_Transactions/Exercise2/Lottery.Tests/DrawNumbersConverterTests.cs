using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Lottery.Domain;
using Lottery.UI.Converters;
using NUnit.Framework;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise02",
        @"Lottery.Data\ConnectionFactory.cs;
Lottery.Data\LotteryGameRepository.cs;
Lottery.Data\DrawRepository.cs;
Lottery.Business\DrawService.cs;
Lottery.UI\LotteryWindow.xaml;
Lottery.UI\LotteryWindow.xaml.cs;
Lottery.UI\Converters\DrawNumbersConverter.cs;")]
    public class DrawNumbersConverterTests
    {
        private DrawNumbersConverter _converter;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _converter = new DrawNumbersConverter();
            _random = new Random();
        }

        [MonitoredTest("DrawNumbersConverter - Convert should create a comma seperated string of numbers"), Order(1)]
        public void _01_Convert_ShouldCreateACommaSeperatedStringOfNumbers()
        {
            //Arrange
            var drawedNumbers = new HashSet<DrawNumber>
            {
                new DrawNumberBuilder(_random).Build(),
                new DrawNumberBuilder(_random).Build(),
                new DrawNumberBuilder(_random).Build()
            };

            //Act
            object result = null;
            try
            {
                result = _converter.Convert(drawedNumbers, null, null, null);
            }
            catch (InvalidCastException)
            {
                Assert.Fail("An InvalidCastException is thrown when the value is a Hashset<DrawNumber>. " +
                            "Cast to an IEnumerable<DrawNumber> to be able to handle all collection types.");
            }

            //Assert
            Assert.That(result, Is.Not.Null, "The converted object should not be null.");
            Assert.That(result, Is.TypeOf<string>(),
                 $"The converted object should be a string, but was '{result.GetType().FullName}'.");
            var numberString = (string)result;
            Assert.That(drawedNumbers.All(d => numberString.Contains(d.Number.ToString())), Is.True,
                 "The converted string does not contain all the numbers.");
            Assert.That(numberString.Count(c => c == ','), Is.EqualTo(drawedNumbers.Count - 1),
                $"The converted string should contain {drawedNumbers.Count - 1} comma's when there are {drawedNumbers.Count} drawed numbers.");
            Assert.That(numberString, Does.Not.EndsWith(" ").And.Not.EndsWith(","),
                 "The converted string should not end with a whitespace or a comma.");
        }

        [MonitoredTest("DrawNumbersConverter - Convert should sort the numbers by position"), Order(2)]
        public void _02_Convert_ShouldSortTheNumbersByPosition()
        {
            //Arrange
            var drawNumber5 = new DrawNumberBuilder(_random).WithPosition(5).Build();
            var drawNumber10 = new DrawNumberBuilder(_random).WithPosition(10).Build();
            var drawNumber1 = new DrawNumberBuilder(_random).WithPosition(1).Build();
            var drawNumberWithoutPosition = new DrawNumberBuilder(_random).Build();
            var drawedNumbers = new Collection<DrawNumber>
            {
                drawNumber5,
                drawNumber10,
                drawNumber1,
                drawNumberWithoutPosition
            };

            //Act
            object result = null;
            try
            {
                result = _converter.Convert(drawedNumbers, null, null, null);
            }
            catch (InvalidCastException)
            {
                Assert.Fail("An InvalidCastException is thrown when the value is a Collection<DrawNumber>. " +
                            "Cast to an IEnumerable<DrawNumber> to be able to handle more collection types.");
            }

            //Assert
            Assert.That(result, Is.Not.Null, () => "The converted object should not be null.");
            Assert.That(result, Is.TypeOf<string>(),
                () => $"The converted object should be a string, but was '{result.GetType().FullName}'.");
            var numberString = (string)result;
            List<int> numbers = null;
            try
            {
                numbers = numberString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => int.Parse(n.Trim())).ToList();
            }
            catch (Exception)
            {
                Assert.Fail("The converted string is not a list of numbers separated by a comma.");
            }

            Assert.That(numbers, Has.Count.EqualTo(drawedNumbers.Count),
                "The converted string does not contain all the numbers.");
            Assert.That(numbers[0], Is.EqualTo(drawNumberWithoutPosition.Number),
                 "Incorrect order. Numbers without a position should be first.");
            Assert.That(numbers[1], Is.EqualTo(drawNumber1.Number), "Incorrect order.");
            Assert.That(numbers[2], Is.EqualTo(drawNumber5.Number), "Incorrect order.");
            Assert.That(numbers[3], Is.EqualTo(drawNumber10.Number), "Incorrect order.");
        }
    }
}