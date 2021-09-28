using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Exercise3.Tests
{
    [ExerciseTestFixture("dotnet2", "H05", "Exercise03", @"Exercise3\IntegerExtensions.cs")]
    public class IntegerExtensionsTests
    {
        private TypeInfo _integerExtensionsTypeInfo;
        private MethodInfo _circularIncrementMethodInfo;
        private Random _random;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _integerExtensionsTypeInfo = typeof(Exercise3.IntegerExtensions).GetTypeInfo();

            if (_integerExtensionsTypeInfo == null) return;

            _circularIncrementMethodInfo = _integerExtensionsTypeInfo.DeclaredMethods.FirstOrDefault(m => m.Name == "CircularIncrement");

            _random = new Random();
        }

        [MonitoredTest("There should be a static IntegerExtensions class"), Order(1)]
        public void _01_ShouldHaveAStaticIntegerExtensionsClass()
        {
            Assert.That(_integerExtensionsTypeInfo, Is.Not.Null, "Cannot find a type with the name 'IntegerExtensions'.");
            Assert.That(_integerExtensionsTypeInfo.IsAbstract && _integerExtensionsTypeInfo.IsSealed, Is.True, "The 'IntegerExtensions' class must be static");
        }

        [MonitoredTest("IntegerExtensions - CircularIncrement - Should be defined correctly"), Order(2)]
        public void _02_CircularIncrement_ShouldBeDefinedCorrectly()
        {
            AssertCircularIncrementMethodIsDefinedCorrectly();
        }

        [MonitoredTest("IntegerExtensions - CircularIncrement - Should return the next value within the range"), Order(3)]
        public void _03_CircularIncrement_ShouldReturnTheNextValueWithinTheRange()
        {
            AssertCircularIncrementMethodIsDefinedCorrectly();

            int minimum = _random.Next(1, 11);
            int maximum = _random.Next(20, 31);

            for (int number = minimum; number < maximum; number++)
            {
                int result = InvokeCircularIncrement(number, minimum, maximum);
                Assert.That(result, Is.EqualTo(number + 1),
                    $"The circular increment of {number} should be {number + 1} when the inclusive minimum is {minimum} and the inclusive maximum is {maximum}.");
            }
        }

        [MonitoredTest("IntegerExtensions - CircularIncrement - Should return the minimum value when next value is not within the range"), Order(4)]
        public void _04_CircularIncrement_ShouldReturnTheMinimumValueWhenNextValueIsNotWithinTheRange()
        {
            AssertCircularIncrementMethodIsDefinedCorrectly();

            int minimum = _random.Next(1, 11);
            int maximum = _random.Next(20, 31);

            int result = InvokeCircularIncrement(minimum - 1, minimum, maximum);
            Assert.That(result, Is.EqualTo(minimum),
                $"The circular increment of {minimum - 1} should be {minimum} when the inclusive minimum is {minimum} and the inclusive maximum is {maximum}.");

            result = InvokeCircularIncrement(minimum - 10, minimum, maximum);
            Assert.That(result, Is.EqualTo(minimum),
                $"The circular increment of {minimum - 10} should be {minimum} when the inclusive minimum is {minimum} and the inclusive maximum is {maximum}.");

            result = InvokeCircularIncrement(maximum, minimum, maximum);
            Assert.That(result, Is.EqualTo(minimum),
                $"The circular increment of {maximum} should be {minimum} when the inclusive minimum is {minimum} and the inclusive maximum is {maximum}.");

            result = InvokeCircularIncrement(maximum + 10, minimum, maximum);
            Assert.That(result, Is.EqualTo(minimum),
                $"The circular increment of {maximum + 10} should be {minimum} when the inclusive minimum is {minimum} and the inclusive maximum is {maximum}.");
        }

        private int InvokeCircularIncrement(int number, int minimum, int maximum)
        {
            return (int)_circularIncrementMethodInfo.Invoke(null, new object[] { number, minimum, maximum });
        }

        private void AssertCircularIncrementMethodIsDefinedCorrectly()
        {
            Assert.That(_circularIncrementMethodInfo, Is.Not.Null, "Cannot find a method with the name 'CircularIncrement'.");
            Assert.That(_circularIncrementMethodInfo.IsStatic, Is.True, "The 'CircularIncrement' method must be static");
            Assert.That(_circularIncrementMethodInfo.ReturnParameter.ParameterType, Is.EqualTo(typeof(int)),
                "The method should return an 'int'.");

            List<ParameterInfo> parameters = _circularIncrementMethodInfo.GetParameters().Where(p => p.ParameterType == typeof(int)).ToList();

            Assert.That(parameters.Count, Is.EqualTo(3), "There should be 3 int parameters: the number being extended, the inclusive minimum and the inclusive maximum.");

            Assert.That(_circularIncrementMethodInfo.GetCustomAttribute<ExtensionAttribute>(), Is.Not.Null,
                "You must use the 'this' keyword in the first method parameter for it to be an extension method");
        }
    }
}