using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise1.Tests
{
    [ExerciseTestFixture("dotnet2", "H05", "Exercise01",
        @"Exercise1\RandomExtensions.cs;
Exercise1\Balloon.cs")]
    public class RandomExtensionsTests
    {
        private TypeInfo _randomExtensionsTypeInfo;
        private MethodInfo _nextBalloonMethodInfo;
        private MethodInfo _nextBalloonFromArrayMethodInfo;
        private Random _random;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            var assembly = Assembly.GetAssembly(typeof(Program));
            _randomExtensionsTypeInfo = assembly.DefinedTypes.FirstOrDefault(t => t.Name == "RandomExtensions");

            if (_randomExtensionsTypeInfo == null) return;

            _nextBalloonMethodInfo = _randomExtensionsTypeInfo.DeclaredMethods.FirstOrDefault(m => m.Name == "NextBalloon");
            _nextBalloonFromArrayMethodInfo = _randomExtensionsTypeInfo.DeclaredMethods.FirstOrDefault(m => m.Name == "NextBalloonFromArray");

            _random = new Random();
        }

        [MonitoredTest("There should be a RandomExtensions class"), Order(1)]
        public void _01_ShouldHaveARandomExtensionsClass()
        {
            Assert.That(_randomExtensionsTypeInfo, Is.Not.Null, "Cannot find a type with the name 'RandomExtensions'.");
            Assert.That(_randomExtensionsTypeInfo.IsAbstract && _randomExtensionsTypeInfo.IsSealed, Is.True, "The 'RandomExtensions' class must be static");
        }

        [MonitoredTest("Balloon - Should not have been changed"), Order(2)]
        public void _02_Balloon_ShouldNotHaveBeenChanged()
        {
            string balloonContentHash = Solution.Current.GetFileHash(@"Exercise1\Balloon.cs");
            Assert.That(balloonContentHash, Is.EqualTo("EB-57-B7-FF-89-18-40-80-A6-BD-75-2B-C1-20-C2-54"));
        }

        [MonitoredTest("RandomExtensions - NextBalloon - Should be defined correctly"), Order(3)]
        public void _03_NextBalloon_ShouldBeDefinedCorrectly()
        {
            AssertNextBalloonMethodIsDefinedCorrectly();
        }

        [MonitoredTest("RandomExtensions - NextBalloon - Should return a Balloon with random size"), Order(4)]
        public void _04_NextBalloon_ShouldReturnBalloonWithRandomSize()
        {
            AssertNextBalloonMethodIsDefinedCorrectly();

            int numberOfBalloons = 100;
            var generatedSizes = new HashSet<int>();
            int numberOfDuplicateSizes = 0;
            for (int i = 0; i < numberOfBalloons; i++)
            {
                Balloon balloon = InvokeNextBalloon(int.MaxValue - 1);
                Assert.That(balloon, Is.Not.Null, "The returned balloon cannot be null");
                if (generatedSizes.Contains(balloon.Size))
                {
                    numberOfDuplicateSizes++;
                }
                else
                {
                    generatedSizes.Add(balloon.Size);
                }
                Assert.That(balloon.Size, Is.GreaterThanOrEqualTo(1), "The balloon size must be greater than or equal to 1");
            }

            if (numberOfDuplicateSizes > numberOfBalloons * 0.2) //20% duplicates with a max size of int.MaxValue is nearly impossible
            {
                Assert.Fail(
                    "The size of the balloon seems not to be random. " +
                    $"{numberOfDuplicateSizes * 100.0 / numberOfBalloons}% of {numberOfBalloons} balloon generations result in te same size.");
            }
        }

        [MonitoredTest("RandomExtensions - NextBalloon - Generated size should be less than or equal to the maximum size"), Order(5)]
        public void _05_NextBalloon_GeneratedSizeShouldBeLessThanOrEqualToTheMaximumSize()
        {
            AssertNextBalloonMethodIsDefinedCorrectly();

            int numberOfBalloons = 30;
            for (int i = 0; i < numberOfBalloons; i++)
            {
                int maxSize = _random.Next(1,4);
                Balloon balloon = InvokeNextBalloon(maxSize);
                Assert.That(balloon, Is.Not.Null, "The returned balloon cannot be null");
                Assert.That(balloon.Size, Is.LessThanOrEqualTo(maxSize),
                    $"Invalid balloon size when invoked with maximum size '{maxSize}'.");
            }
        }

        [MonitoredTest("RandomExtensions - Should return a Balloon with a random color"), Order(6)]
        public void _06_NextBalloon_ShouldReturnBalloonWithRandomColor()
        {
            AssertNextBalloonMethodIsDefinedCorrectly();

            int numberOfBalloons = 100;
            var generatedColors = new HashSet<Color>();
            int numberOfDuplicateColors = 0;
            for (int i = 0; i < numberOfBalloons; i++)
            {
                Balloon balloon = InvokeNextBalloon(int.MaxValue - 1);
                Assert.That(balloon, Is.Not.Null, "The returned balloon cannot be null");
                if (generatedColors.Contains(balloon.Color))
                {
                    numberOfDuplicateColors++;
                }
                else
                {
                    generatedColors.Add(balloon.Color);
                }
            }

            if (numberOfDuplicateColors > numberOfBalloons * 0.2) //20% duplicate colors is nearly impossible
            {
                Assert.Fail(
                    "The color of the balloon seems not to be random. " +
                    $"{numberOfDuplicateColors * 100.0 / numberOfBalloons}% of {numberOfBalloons} balloon generations result in te same color." +
                    "\nTip: use the static 'FromArgb(Int32, Int32, Int32)' method of the 'Color' class to create a color.");
            }
        }

        [MonitoredTest("RandomExtensions - NextBalloonFromArray - Should be defined correctly"), Order(7)]
        public void _07_NextBalloonFromArray_ShouldBeDefinedCorrectly()
        {
            AssertNextBalloonFromArrayMethodIsDefinedCorrectly();
        }

        [MonitoredTest("RandomExtensions - NextBalloonFromArray - Should return a random item from the Balloon array"), Order(8)]
        public void _08_NextBalloonFromArray_ShouldReturnRandomItemFromBalloonArray()
        {
            AssertNextBalloonFromArrayMethodIsDefinedCorrectly();

            int numberOfBalloons = 10000;
            Balloon[] allBalloons = new Balloon[numberOfBalloons];
            for (int i = 0; i < numberOfBalloons; i++)
            {
                allBalloons[i] = new Balloon(Color.Red, 10);
            }

            var pickedBalloons = new HashSet<Balloon>();
            int numberOfPicks = 100;
            int numberOfDuplicatePicks = 0;
            for (int i = 0; i < numberOfPicks; i++)
            {
                Balloon balloon = InvokeNextBalloonFromArray(allBalloons);
                Assert.That(balloon, Is.Not.Null, "The returned balloon cannot be null");
                Assert.That(allBalloons.Any(b => b == balloon), Is.True, "The returned balloon must be a balloon from the array.");
                if (pickedBalloons.Contains(balloon))
                {
                    numberOfDuplicatePicks++;
                }
                else
                {
                    pickedBalloons.Add(balloon);
                }
            }

            if (numberOfDuplicatePicks > numberOfPicks * 0.2) //20% duplicate picks on 10000 balloons is nearly impossible
            {
                Assert.Fail(
                    "The picking of a balloon seems not to be random. " +
                    $"{numberOfDuplicatePicks * 100.0 / numberOfPicks}% of {numberOfPicks} balloon picks " +
                    $"in a balloon array of size {numberOfBalloons} pick an already picked balloon");
            }
        }

        private Balloon InvokeNextBalloon(int maxSize)
        {
            return (Balloon)_nextBalloonMethodInfo.Invoke(null, new object[] { _random, maxSize });
        }

        private Balloon InvokeNextBalloonFromArray(Balloon[] balloons)
        {
            return (Balloon)_nextBalloonFromArrayMethodInfo.Invoke(null, new object[] { _random, balloons });
        }

        private void AssertNextBalloonMethodIsDefinedCorrectly()
        {
            Assert.That(_nextBalloonMethodInfo, Is.Not.Null, "Cannot find a method with the name 'NextBalloon'.");
            Assert.That(_nextBalloonMethodInfo.IsStatic, Is.True, "The 'NextBalloon' method must be static");
            Assert.That(_nextBalloonMethodInfo.ReturnParameter.ParameterType, Is.EqualTo(typeof(Balloon)),
                "The method should return a 'Balloon'.");

            var parameters = _nextBalloonMethodInfo.GetParameters();

            var randomParameter = parameters.FirstOrDefault(p => p.ParameterType == typeof(Random));
            Assert.That(randomParameter, Is.Not.Null, "Cannot find a parameter of type 'Random'.");

            var maxSizeParameter = parameters.FirstOrDefault(p => p.ParameterType == typeof(int));
            Assert.That(maxSizeParameter, Is.Not.Null,
                "Cannot find a parameter of type 'int' that is used to pass in the maximum size.");

            Assert.That(_nextBalloonMethodInfo.GetCustomAttribute<ExtensionAttribute>(), Is.Not.Null,
                "You must use the 'this' keyword for the method to be an extension method");
        }

        private void AssertNextBalloonFromArrayMethodIsDefinedCorrectly()
        {
            Assert.That(_nextBalloonFromArrayMethodInfo, Is.Not.Null, "Cannot find a method with the name 'NextBalloonFromArray'.");
            Assert.That(_nextBalloonFromArrayMethodInfo.IsStatic, Is.True, "The 'NextBalloonFromArray' method must be static");
            Assert.That(_nextBalloonFromArrayMethodInfo.ReturnParameter.ParameterType, Is.EqualTo(typeof(Balloon)),
                "The method should return a 'Balloon'.");

            var parameters = _nextBalloonFromArrayMethodInfo.GetParameters();

            var randomParameter = parameters.FirstOrDefault(p => p.ParameterType == typeof(Random));
            Assert.That(randomParameter, Is.Not.Null, "Cannot find a parameter of type 'Random'.");

            var balloonArrayParameter = parameters.FirstOrDefault(p => p.ParameterType == typeof(Balloon[]));
            Assert.That(balloonArrayParameter, Is.Not.Null,
                "Cannot find a parameter of type 'Balloon[]'. This parameter should be the array of balloons from which a random item is picked.");

            Assert.That(_nextBalloonFromArrayMethodInfo.GetCustomAttribute<ExtensionAttribute>(), Is.Not.Null,
                "You must use the 'this' keyword for the method to be an extension method");
        }
    }
}
