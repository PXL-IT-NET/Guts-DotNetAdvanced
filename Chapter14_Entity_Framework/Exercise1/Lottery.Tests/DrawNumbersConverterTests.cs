using System;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Lottery.Domain;
using Lottery.UI.Converters;
using NUnit.Framework;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise01",
        @"Lottery.Data\LotteryContext.cs;Lottery.Data\LotteryGameRepository.cs;Lottery.Data\DrawRepository.cs;Lottery.Business\DrawService.cs;Lottery.UI\LotteryWindow.xaml;Lottery.UI\LotteryWindow.xaml.cs;Lottery.UI\Converters\DrawNumbersConverter.cs;")]
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
            var drawedNumbers = new List<DrawNumber>
            {
                new DrawNumberBuilder(_random).Build(),
                new DrawNumberBuilder(_random).Build(),
                new DrawNumberBuilder(_random).Build()
            };

            //Act
            var result = _converter.Convert(drawedNumbers, null, null, null);

            //Assert
            Assert.That(result, Is.Not.Null, () => "The converted object should not be null.");
            Assert.That(result, Is.TypeOf<string>(),
                () => $"The converted object should be a string, but was '{result.GetType().FullName}'.");
            var numberString = (string)result;
            Assert.That(drawedNumbers.All(d => numberString.Contains(d.Number.ToString())), Is.True,
                () => "The converted string does not contain all the numbers.");
            Assert.That(numberString.Count(c => c == ','), Is.EqualTo(drawedNumbers.Count - 1),
                () =>
                    $"The converted string should contain {drawedNumbers.Count - 1} comma's when there are {drawedNumbers.Count} drawed numbers.");
            Assert.That(numberString, Does.Not.EndsWith(" ").And.Not.EndsWith(","),
                () => "The converted string should not end with a whitespace or a comma.");
        }

        [MonitoredTest("DrawNumbersConverter - Convert should sort the numbers by position"), Order(2)]
        public void _02_Convert_ShouldSortTheNumbersByPosition()
        {
            //Arrange
            var drawedNumbers = new List<DrawNumber>
            {
                new DrawNumberBuilder(_random).WithPosition(5).Build(),
                new DrawNumberBuilder(_random).WithPosition(10).Build(),
                new DrawNumberBuilder(_random).WithPosition(1).Build(),
                new DrawNumberBuilder(_random).Build()
            };

            //Act
            var result = _converter.Convert(drawedNumbers, null, null, null);

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
                Assert.Fail("The converted string is not a list of numbers seperated by a comma.");
            }

            Assert.That(numbers, Has.Count.EqualTo(drawedNumbers.Count),
                () => "The converted string does not contain all the numbers.");
            Assert.That(numbers[0], Is.EqualTo(drawedNumbers[3].Number),
                () => "Incorrect order. Numbers without a position should be first.");
            Assert.That(numbers[1], Is.EqualTo(drawedNumbers[2].Number), () => "Incorrect order.");
            Assert.That(numbers[2], Is.EqualTo(drawedNumbers[0].Number), () => "Incorrect order.");
            Assert.That(numbers[3], Is.EqualTo(drawedNumbers[1].Number), () => "Incorrect order.");
        }
    }
}