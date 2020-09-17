using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NumberConverter.UI.Tests;
using NUnit.Framework;

namespace Guts.Tests
{
    [ExerciseTestFixture("dotnet2", "H05", "Exercise01",
        @"NumberConverter.UI\Converters\RomanNumberConverter.cs;NumberConverter.UI.Tests\RomanNumberConverterTests.cs")]
    public class RomanNumberConverterTestsTests
    {
        private const string ValueShouldBeOfStringTypeMethodName = "Convert_ShouldThrowArgumentExceptionWhenValueIsNotAString";
        private const string ValueCanBeParsedToIntMethodName = "Convert_ShouldReturnInvalidNumberWhenTheValueCannotBeParsedAsAnInteger";
        private const string ValueShouldBeInRangeMethodName = "Convert_ShouldReturnOutOfRangeWhenTheValueIsNotBetweeOneAnd3999";
        private const string ValidValueShouldConvertCorrectlyMethodName = "Convert_ShouldCorrectlyConvertValidNumbers";

        private TestFixtureAttribute _testFixtureAttribute;
        private MethodInfo _setupMethod;
        private MethodInfo _valueShouldBeOfStringTypeMethod;
        private MethodInfo _valueCanBeParsedToIntMethod;
        private MethodInfo _valueShouldBeInRangeMethod;
        private MethodInfo _validValueShouldConvertCorrectlyMethod;
        private string _testClassContent;
        private RomanNumberConverterTests _romanNumberConverterTestsInstance;
        private Random _random;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var testClassType = typeof(RomanNumberConverterTests);

            _testFixtureAttribute = testClassType.GetCustomAttribute<TestFixtureAttribute>();

            _setupMethod = testClassType.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<SetUpAttribute>() != null);

            _valueShouldBeOfStringTypeMethod =
                testClassType.GetMethod(ValueShouldBeOfStringTypeMethodName);

            _valueCanBeParsedToIntMethod =
                testClassType.GetMethod(ValueCanBeParsedToIntMethodName);

            _valueShouldBeInRangeMethod =
                testClassType.GetMethod(ValueShouldBeInRangeMethodName);

            _validValueShouldConvertCorrectlyMethod =
                testClassType.GetMethod(ValidValueShouldConvertCorrectlyMethodName);

            _testClassContent = Solution.Current.GetFileContent(@"NumberConverter.UI.Tests\RomanNumberConverterTests.cs");

            _random = new Random();
        }

        [SetUp]
        public void SetUp()
        {
            _romanNumberConverterTestsInstance = new RomanNumberConverterTests();

            if (_setupMethod != null)
            {
                _setupMethod.Invoke(_romanNumberConverterTestsInstance, new object[0]);
            }
        }

        [MonitoredTest("Should be a TestFixture"), Order(1)]
        public void _01_ShouldBeATestFixture()
        {
            Assert.That(_testFixtureAttribute, Is.Not.Null, "The test class must be marked as a 'TestFixture'.");
        }

        #region String type test

        [MonitoredTest("Should have a test that checks if only string values are accepted"), Order(2)]
        public void _02_ShouldHaveATestThatChecksIfOnlyStringValuesAreAccepted()
        {
            AssertHasValueShouldBeOfStringTypeTestMethod();

            var methodBody = GetMethodBodyWithoutComments(ValueShouldBeOfStringTypeMethodName);
            var methodBodyLower = methodBody.ToLower();

            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);
            Assert.That(methodBodyLower, Contains.Substring("new object()"),
                "The method should use a 'new object()' as the value to convert.");
            Assert.That(methodBody, Contains.Substring("() =>"),
                "The method should use a lambda expression to call the 'Convert' method.");
            Assert.That(methodBody, Contains.Substring("Throws.ArgumentException")
                    .Or.Contains("Throws.InstanceOf<ArgumentException>()"),
                "The method should check if an 'ArgumentException' is thrown.");
            Assert.That(methodBody, Contains.Substring(".With.Message.Contains(").And.Contains("string"),
                "The method should check if the 'Message' of the 'ArgumentException' contains the word 'string'.");
        }

        [MonitoredTest("The test that checks of only string values are accepted should pass"), Order(3)]
        public void _03_TheTestThatChecksIfOnlyStringValuesAreAcceptedShouldPass()
        {
            AssertHasValueShouldBeOfStringTypeTestMethod();
            AssertTestMethodPasses(_valueShouldBeOfStringTypeMethod);
        }
        #endregion

        #region Can parse to int test

        [MonitoredTest("Should have a test that checks if the value can be parsed to an integer"), Order(4)]
        public void _04_ShouldHaveATestThatChecksTheValueCanBeParsedToAnInteger()
        {
            AssertHasValueCanBeParsedToIntTestMethod();

            var testCaseAttributes = _valueCanBeParsedToIntMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            Assert.That(testCaseAttributes, Has.Count.GreaterThanOrEqualTo(3), () => "The method should have at least 3 test cases.");

            Assert.That(testCaseAttributes,
                Has.All.Matches((TestCaseAttribute testCase) => testCase.Arguments?.Length == 1),
                () => "All test cases must have 1 argument (value)");

            var emptyStringTestCase = testCaseAttributes.FirstOrDefault(a => a.Arguments[0] as string == string.Empty);
            Assert.That(emptyStringTestCase, Is.Not.Null,
                "The method should have one 'TestCase' for an empty string value.");

            var nonNumberTestCase = testCaseAttributes.FirstOrDefault(a =>
            {
                if (!(a.Arguments[0] is string value)) return false;
                if (string.IsNullOrEmpty(value)) return false;
                return value.ToCharArray().All(c => !char.IsDigit(c));
            });
            Assert.That(nonNumberTestCase, Is.Not.Null,
                "The method should have one 'TestCase' for a value that does not contain numbers.");

            var methodBody = GetMethodBodyWithoutComments(ValueCanBeParsedToIntMethodName);

            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);

            Assert.That(methodBody, Contains.Substring("\"Invalid number\""),
                "The method should check if the converted result is exactly equal to the string 'Invalid number'.");
        }

        [MonitoredTest("The test that checks if the value can be parsed to an integer should pass"), Order(5)]
        public void _05_TheTestThatChecksTheValueCanBeParsedToAnIntegerShouldPass()
        {
            AssertHasValueCanBeParsedToIntTestMethod();
            AssertTestMethodPasses(_valueCanBeParsedToIntMethod);
        }

        [MonitoredTest("The test that checks if the value can be parsed to an integer should pass for correct expectations"), Order(6)]
        public void _06_TheTestThatChecksTheValueCanBeParsedToAnIntegerShouldPassForCorrectExpectations()
        {
            AssertHasValueCanBeParsedToIntTestMethod();
            AssertTestMethodPasses(_valueCanBeParsedToIntMethod, Guid.NewGuid().ToString());
            AssertTestMethodPasses(_valueCanBeParsedToIntMethod, "1,5");
            AssertTestMethodPasses(_valueCanBeParsedToIntMethod, "");
        }

        [MonitoredTest("The test that checks if the value can be parsed to an integer should fail for wrong expectations"), Order(7)]
        public void _07_TheTestThatChecksTheValueCanBeParsedToAnIntegerShouldFailForWrongExpectation()
        {
            AssertHasValueCanBeParsedToIntTestMethod();

            foreach (var validValue in GetSomeValidValues(5))
            {
                AssertTestMethodFails(_valueCanBeParsedToIntMethod, validValue);
            }
        }
        #endregion

        #region In range test

        [MonitoredTest("Should have a test that checks if the value is in range"), Order(8)]
        public void _08_ShouldHaveATestThatChecksIfTheValueIsInRange()
        {
            AssertHasValueIsInRangeTestMethod();

            var testCaseAttributes = _valueShouldBeInRangeMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            Assert.That(testCaseAttributes, Has.Count.GreaterThanOrEqualTo(2), () => "The method should have at least 2 test cases.");

            Assert.That(testCaseAttributes,
                Has.All.Matches((TestCaseAttribute testCase) => testCase.Arguments?.Length == 1),
                () => "All test cases must have 1 argument (value)");

            var hasInvalidTestCases = testCaseAttributes.Any(a =>
            {
                if (!(a.Arguments[0] is string value)) return true;
                if (string.IsNullOrEmpty(value)) return true;
                if (!int.TryParse(value, out int valueAsNumber)) return true;
                if (valueAsNumber >= 1 && valueAsNumber <= 3999) return true;
                return false;
            });
            Assert.That(hasInvalidTestCases, Is.False,
                "The method should have only 'TestCases' for values that can be parsed to an integer outside the 1-3999 range.");

            var methodBody = GetMethodBodyWithoutComments(ValueShouldBeInRangeMethodName);

            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);

            Assert.That(methodBody, Contains.Substring("\"Out of Roman range (1-3999)\"").IgnoreCase,
                "The method should check if the converted result is exactly equal to the string 'Out of Roman range (1-3999)'.");
        }

        [MonitoredTest("The test that checks if the value is in range should pass"), Order(9)]
        public void _09_TheTestThatChecksIfTheValueIsInRangeShouldPass()
        {
            AssertHasValueIsInRangeTestMethod();
            AssertTestMethodPasses(_valueShouldBeInRangeMethod);
        }

        [MonitoredTest("The test that checks if the value is in range should pass for correct expectations"), Order(10)]
        public void _10_TheTestThatChecksIfTheValueIsInRangeShouldPassForCorrectExpectations()
        {
            AssertHasValueIsInRangeTestMethod();
            AssertTestMethodPasses(_valueShouldBeInRangeMethod, "0");
            AssertTestMethodPasses(_valueShouldBeInRangeMethod, "4000");
            int tooBig = _random.Next(4001, int.MaxValue);
            AssertTestMethodPasses(_valueShouldBeInRangeMethod, tooBig.ToString());
            int tooSmall = -1 * _random.Next(1, int.MaxValue);
            AssertTestMethodPasses(_valueShouldBeInRangeMethod, tooSmall.ToString());
        }

        [MonitoredTest("The test that checks if the value is in range should fail for wrong expectations"), Order(11)]
        public void _11_TheTestThatChecksIfTheValueIsInRangeShouldFailForWrongExpectation()
        {
            AssertHasValueIsInRangeTestMethod();

            foreach (var validValue in GetSomeValidValues(5))
            {
                AssertTestMethodFails(_valueShouldBeInRangeMethod, validValue);
            }

            AssertTestMethodFails(_valueShouldBeInRangeMethod, Guid.NewGuid().ToString());
        }

        #endregion

        #region Correctly convert test

        [MonitoredTest("Should have a test that checks conversion of valid numbers"), Order(12)]
        public void _12_ShouldHaveATestThatChecksConversionOfValidNumbers()
        {
            AssertHasCorrectlyConvertTestMethod();

            var testCaseAttributes = _validValueShouldConvertCorrectlyMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            Assert.That(testCaseAttributes, Has.Count.GreaterThanOrEqualTo(4), () => "The method should have at least 4 test cases.");

            Assert.That(testCaseAttributes,
                Has.All.Matches((TestCaseAttribute testCase) => testCase.Arguments?.Length == 2),
                () => "All test cases must have 2 arguments (value, expected)");

            var allTestCaseValuesAreValid = testCaseAttributes.All(a =>
            {
                if (!(a.Arguments[0] is string value)) return false;
                if (string.IsNullOrEmpty(value)) return false;
                if (!int.TryParse(value, out int valueAsNumber)) return false;

                if (!(a.Arguments[1] is string expected)) return false;
                if (string.IsNullOrEmpty(expected)) return false;

                if (valueAsNumber >= 1 && valueAsNumber <= 3999) return true;

                return false;
            });
            Assert.That(allTestCaseValuesAreValid, Is.True,
                "The method should have only 'TestCases' with the first argument a string that can be parsed to an int inside the 1-3999 range.");

            string validRomanCharacters = "IVXLCDM";
            var allTestCaseExpectationsAreValid = testCaseAttributes.All(a =>
            {
                if (!(a.Arguments[1] is string expected)) return false;
                if (string.IsNullOrEmpty(expected)) return false;

                var hasOnlyRomanCharacters = expected.ToCharArray().All(c => validRomanCharacters.Contains(c));
                return hasOnlyRomanCharacters;
            });
            Assert.That(allTestCaseExpectationsAreValid, Is.True,
                "The method should have only 'TestCases' with the second argument being a valid Roman number. " +
                "A Roman number has no whitespaces and is build using one or more of the following characters: " +
                "I, V, X, L, C, D and M");


            var methodBody = GetMethodBodyWithoutComments(ValidValueShouldConvertCorrectlyMethodName);

            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);

            Assert.That(methodBody, Contains.Substring("Is.EqualTo("),
                "The method should compare the converted result with the expected result'.");
        }

        [MonitoredTest("The test that checks conversion of valid numbers should pass"), Order(13)]
        public void _13_TheTestThatChecksConversionOfValidNumbersShouldPass()
        {
            AssertHasCorrectlyConvertTestMethod();
            AssertTestMethodPasses(_validValueShouldConvertCorrectlyMethod);
        }

        [MonitoredTest("The test that checks conversion of valid numbers should pass for correct expectations"), Order(14)]
        public void _14_TheTestThatChecksConversionOfValidNumbersShouldPassForCorrectExpectations()
        {
            AssertHasCorrectlyConvertTestMethod();

            foreach (var validValue in GetSomeValidValues(100))
            {
                AssertTestMethodPasses(_validValueShouldConvertCorrectlyMethod, validValue, R(Convert.ToInt32(validValue)));
            }
        }

        [MonitoredTest("The test that checks conversion of valid numbers should fail for wrong expectations"), Order(15)]
        public void _15_TheTestThatChecksConversionOfValidNumbersShouldFailForWrongExpectation()
        {
            AssertHasCorrectlyConvertTestMethod();

            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "2", "2");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "2", "ii");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "3", "II");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "4", "IIII");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "4", "iv");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "9", "VIIII");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "10", "VV");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "30", "XXXX");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "40", "XXXX");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "60", "XL");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "90", "LXXXX");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "400", "CCCC");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "500", "CCCCC");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "900", "DCCCC");
            AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, "1000", "DD");

            foreach (var validValue in GetSomeValidValues(10))
            {
                AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, validValue, Guid.NewGuid().ToString());
                AssertTestMethodFails(_validValueShouldConvertCorrectlyMethod, validValue, "");
            }
        }

        #endregion

        #region Helpers
        private IList<string> GetSomeValidValues(int numberOfValues)
        {
            var values = new List<string>();
            for (int i = 0; i < numberOfValues; i++)
            {
                values.Add(_random.Next(1, 4000).ToString());
            }
            return values;
        }

        private void AssertCallsSutMethodAndUsesAssertThatSyntax(string methodBody)
        {
            Assert.That(methodBody, Contains.Substring(".Convert("),
                "The method should call the 'Convert' method of a 'RomanNumberConverter' instance.");
            Assert.That(methodBody, Contains.Substring("Assert.That("),
                "The method should use the 'Assert.That' method of NUnit.");
        }

        private void AssertHasValueShouldBeOfStringTypeTestMethod()
        {
            AssertHasTestMethod(
                _valueShouldBeOfStringTypeMethod, ValueShouldBeOfStringTypeMethodName, 0);
        }

        private void AssertHasValueCanBeParsedToIntTestMethod()
        {
            AssertHasTestMethod(
                _valueCanBeParsedToIntMethod, ValueCanBeParsedToIntMethodName, 1);

            var parameter = _valueCanBeParsedToIntMethod.GetParameters().First();
            Assert.That(parameter.ParameterType, Is.EqualTo(typeof(string)),
                $"The parameter of the '{ValueCanBeParsedToIntMethodName}' method should be of type 'string'.");
        }

        private string R(int a)
        {
            if (a >= 1000) return "M" + R(a - 1000);
            if (a >= 900) return "CM" + R(a - 900);
            if (a >= 500) return "D" + R(a - 500);
            if (a >= 400) return "CD" + R(a - 400);
            if (a >= 100) return "C" + R(a - 100);
            if (a >= 90) return "XC" + R(a - 90);
            if (a >= 50) return "L" + R(a - 50);
            if (a >= 40) return "XL" + R(a - 40);
            if (a >= 10) return "X" + R(a - 10);
            if (a >= 9) return "IX" + R(a - 9);
            if (a >= 5) return "V" + R(a - 5);
            if (a >= 4) return "IV" + R(a - 4);
            if (a >= 1) return "I" + R(a - 1);
            return string.Empty;
        }

        private void AssertHasValueIsInRangeTestMethod()
        {
            AssertHasTestMethod(
                _valueShouldBeInRangeMethod, ValueShouldBeInRangeMethodName, 1);

            var parameter = _valueCanBeParsedToIntMethod.GetParameters().First();
            Assert.That(parameter.ParameterType, Is.EqualTo(typeof(string)),
                $"The parameter of the '{ValueShouldBeInRangeMethodName}' method should be of type 'string'.");
        }

        private void AssertHasCorrectlyConvertTestMethod()
        {
            AssertHasTestMethod(
                _validValueShouldConvertCorrectlyMethod, ValidValueShouldConvertCorrectlyMethodName, 2);

            var parameters = _valueCanBeParsedToIntMethod.GetParameters().ToList();
            Assert.That(parameters, Has.All.Matches((ParameterInfo p) => p.ParameterType == typeof(string)),
                $"The parameters of the '{ValidValueShouldConvertCorrectlyMethodName}' method should all be of type 'string'.");
        }

        private void AssertHasTestMethod(MethodInfo testMethod, string methodName, int expectedParameterCount)
        {
            Assert.That(testMethod, Is.Not.Null,
                $"Could not find test method with name '{methodName}'.");

            Assert.That(testMethod.GetParameters().Length, Is.EqualTo(expectedParameterCount),
                 $"The method '{methodName}' should have {expectedParameterCount} parameter(s).");

            if (expectedParameterCount == 0)
            {
                var testAttribute = testMethod.GetCustomAttributes()
                    .OfType<TestAttribute>().FirstOrDefault();
                Assert.That(testAttribute, Is.Not.Null,
                    "No 'Test' attribute is defined for the method.");
            }
        }

        private string GetMethodBodyWithoutComments(string methodName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_testClassContent);
            var root = syntaxTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals(methodName));

            if (method == null) return string.Empty;

            var bodyBuilder = new StringBuilder(); //no pun intended :)
            foreach (var statement in method.Body.Statements)
            {
                bodyBuilder.AppendLine(statement.ToString());
            }
            return bodyBuilder.ToString();
        }

        private void AssertTestMethodFails(MethodInfo testMethod, params object[] parameters)
        {
            Assert.That(() => testMethod.Invoke(_romanNumberConverterTestsInstance, parameters), Throws.InstanceOf<Exception>(),
                () => $"{testMethod.Name}({StringyfyParameters(parameters)}) should fail, but doesn't.");
        }

        private void AssertTestMethodPasses(MethodInfo testMethod)
        {
            var testCaseAttributes = testMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            if (testCaseAttributes.Any())
            {
                foreach (var testCaseAttribute in testCaseAttributes)
                {
                    AssertTestMethodPasses(testMethod, testCaseAttribute.Arguments);
                }
            }
            else
            {
                AssertTestMethodPasses(testMethod, new object[0]);
            }
        }

        private void AssertTestMethodPasses(MethodInfo testMethod, params object[] parameters)
        {
            Assert.That(() => testMethod.Invoke(_romanNumberConverterTestsInstance, parameters), Throws.Nothing,
                () => $"{testMethod.Name}({StringyfyParameters(parameters)}) should pass, but doesn't.");
        }

        private string StringyfyParameters(params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return string.Empty;

            var builder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                if (parameter is string)
                {
                    builder.Append($"\"{parameter}\"");
                }
                else
                {
                    builder.Append(parameter);
                }

                builder.Append(", ");
            }

            builder.Remove(builder.Length - 2, 2);
            return builder.ToString();
        }
        #endregion
    }
}
