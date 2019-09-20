using FizzBuzz.Business.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FizzBuzz.Business;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Guts.Tests
{
    [ExerciseTestFixture("dotnet2", "H05", "Exercise02",
        @"FizzBuzz.Business\FizzBuzzService.cs;FizzBuzz.Business.Tests\FizzBuzzServiceTests.cs")]
    public class FizzBuzzServiceTestsTests
    {
        private const string GenerateFizzBuzzWithCorrectParamtersTestMethodName = "ReturnsCorrectFizzBuzzTextWhenParametersAreValid";
        private const string CheckExceptionThrowingWhenFizzIsOutOfRangeMethodName = "ThrowsValidationExceptionWhenFizzFactorIsNotInRange";
        private const string CheckExceptionThrowingWhenBuzzIsOutOfRangeMethodName = "ThrowsValidationExceptionWhenBuzzFactorIsNotInRange";
        private const string CheckExceptionThrowingWhenLastNumberIsOutOfRangeMethodName = "ThrowsValidationExceptionWhenLastNumberIsNotInRange";

        private MethodInfo _generateFizzBuzzWithCorrectParamtersTestMethod;
        private FizzBuzzServiceTests _fizzBuzzTestsInstance;
        private MethodInfo _setupMethod;
        private TestFixtureAttribute _testFixtureAttribute;
        private MethodInfo _checkExceptionThrowingWhenFizzIsOutOfRangTestMethod;
        private MethodInfo _checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod;
        private MethodInfo _checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod;
        private string _testClassContent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var testClassType = typeof(FizzBuzzServiceTests);

            _testFixtureAttribute = testClassType.GetCustomAttribute<TestFixtureAttribute>();

            _setupMethod = testClassType.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<SetUpAttribute>() != null);

            _generateFizzBuzzWithCorrectParamtersTestMethod =
                testClassType.GetMethod(GenerateFizzBuzzWithCorrectParamtersTestMethodName);

            _checkExceptionThrowingWhenFizzIsOutOfRangTestMethod =
                testClassType.GetMethod(CheckExceptionThrowingWhenFizzIsOutOfRangeMethodName);

            _checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod =
                testClassType.GetMethod(CheckExceptionThrowingWhenBuzzIsOutOfRangeMethodName);

            _checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod =
                testClassType.GetMethod(CheckExceptionThrowingWhenLastNumberIsOutOfRangeMethodName);

            _testClassContent = Solution.Current.GetFileContent(@"FizzBuzz.Business.Tests\FizzBuzzServiceTests.cs");
        }

        [SetUp]
        public void SetUp()
        {
            _fizzBuzzTestsInstance = new FizzBuzzServiceTests();

            if (_setupMethod != null)
            {
                _setupMethod.Invoke(_fizzBuzzTestsInstance, new object[0]);
            }
        }

        [MonitoredTest("Should be a TestFixture"), Order(1)]
        public void _01_ShouldBeATestFixture()
        {
            Assert.That(_testFixtureAttribute, Is.Not.Null, () => "The test class must be marked as a 'TestFixture'.");
        }

        [MonitoredTest("Should have a test that checks FizzBuzz generation for valid parameters"), Order(2)]
        public void _02_ShouldHaveATestThatChecksFizzBuzzGenerationForValidParameters()
        {
            AssertHasGenerateFizzBuzzWithCorrectParamtersTestMethod();

            var testCaseAttributes = _generateFizzBuzzWithCorrectParamtersTestMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            Assert.That(testCaseAttributes, Has.Count.GreaterThanOrEqualTo(5), () => "The method should have at least 5 test cases.");

            Assert.That(testCaseAttributes,
                Has.All.Matches((TestCaseAttribute testCase) => testCase.Arguments?.Length == 4),
                () => "All test cases must have 4 arguments (fizzFactor, buzzFactor, lastNumber, expected)");

            var methodBody = GetMethodBodyWithoutComments(GenerateFizzBuzzWithCorrectParamtersTestMethodName);
            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);
        }

        [MonitoredTest("The test that checks FizzBuzz generation for valid parameters should pass"), Order(3)]
        public void _03_TheTestThatChecksFizzBuzzGenerationForValidParamtersShouldPass()
        {
            AssertHasGenerateFizzBuzzWithCorrectParamtersTestMethod();
            AssertTestMethodPasses(_generateFizzBuzzWithCorrectParamtersTestMethod);
        }

        [MonitoredTest("The test that checks FizzBuzz generation for valid parameters should pass for correct expectations"), Order(4)]
        public void _04_TheTestThatChecksFizzBuzzGenerationForValidParamtersShouldPassForCorrectExpectations()
        {
            AssertHasGenerateFizzBuzzWithCorrectParamtersTestMethod();
            AssertTestMethodPasses(_generateFizzBuzzWithCorrectParamtersTestMethod, 2, 3, 1, "1");
            AssertTestMethodPasses(_generateFizzBuzzWithCorrectParamtersTestMethod, 4, 5, 4, "1 2 3 Fizz");
            AssertTestMethodPasses(_generateFizzBuzzWithCorrectParamtersTestMethod, 5, 4, 4, "1 2 3 Buzz");
            AssertTestMethodPasses(_generateFizzBuzzWithCorrectParamtersTestMethod, 2, 3, 15,
                "1 Fizz Buzz Fizz 5 FizzBuzz 7 Fizz Buzz Fizz 11 FizzBuzz 13 Fizz Buzz");
            AssertTestMethodPasses(_generateFizzBuzzWithCorrectParamtersTestMethod, 2, 2, 4, "1 FizzBuzz 3 FizzBuzz");
        }

        [MonitoredTest("The test that checks FizzBuzz generation for valid parameters should fail for wrong expectations"), Order(5)]
        public void _05_TheTestThatChecksFizzBuzzGenerationForValidParamtersShouldFailForWrongExpectation()
        {
            AssertHasGenerateFizzBuzzWithCorrectParamtersTestMethod();
            AssertTestMethodFails(_generateFizzBuzzWithCorrectParamtersTestMethod, 2, 3, 1, "");
            AssertTestMethodFails(_generateFizzBuzzWithCorrectParamtersTestMethod, 4, 5, 4, "1 2 3 4");
            AssertTestMethodFails(_generateFizzBuzzWithCorrectParamtersTestMethod, 5, 4, 4, "1 2 3 FizzBuzz");
            AssertTestMethodFails(_generateFizzBuzzWithCorrectParamtersTestMethod, 2, 2, 4, "1 Fizz 3 Fizz");
        }

        [MonitoredTest("Should have a test that checks exception throwing when Fizz is out of range"), Order(6)]
        public void _06_ShouldHaveATestThatChecksExceptionThrowingWhenFizzIsOutOfRange()
        {
            AssertHasCheckExceptionThrowingWhenFizzIsOutOfRangeTestMethod();

            var testCaseAttributes = _checkExceptionThrowingWhenFizzIsOutOfRangTestMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            AssertFactorOutOfRangeTestCases(testCaseAttributes, "fizzFactor");

            var methodBody = GetMethodBodyWithoutComments(CheckExceptionThrowingWhenFizzIsOutOfRangeMethodName);
            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);
            AssertUsesThrowsForFizzBuzzValidationException(methodBody);
        }

        [MonitoredTest("The test that checks exception throwing when Fizz is out of range should pass"), Order(7)]
        public void _07_TheTestThatChecksExceptionThrowingWhenFizzIsOutOfRangeShouldPass()
        {
            AssertHasCheckExceptionThrowingWhenFizzIsOutOfRangeTestMethod();
            AssertTestMethodPasses(_checkExceptionThrowingWhenFizzIsOutOfRangTestMethod);
        }

        [MonitoredTest("The test that checks exception throwing when Fizz is out of range should fail when in range"), Order(8)]
        public void _08_TheTestThatChecksExceptionThrowingWhenFizzIsOutOfRangeShouldFailWhenInRange()
        {
            AssertHasCheckExceptionThrowingWhenFizzIsOutOfRangeTestMethod();
            AssertTestMethodFails(_checkExceptionThrowingWhenFizzIsOutOfRangTestMethod, FizzBuzzService.MinimumFactor);
            var averageFactor = (FizzBuzzService.MinimumFactor + FizzBuzzService.MaximumFactor) / 2;
            AssertTestMethodFails(_checkExceptionThrowingWhenFizzIsOutOfRangTestMethod, averageFactor);
            AssertTestMethodFails(_checkExceptionThrowingWhenFizzIsOutOfRangTestMethod, FizzBuzzService.MaximumFactor);
        }

        [MonitoredTest("Should have a test that checks exception throwing when Buzz is out of range"), Order(9)]
        public void _09_ShouldHaveATestThatChecksExceptionThrowingWhenBuzzIsOutOfRange()
        {
            AssertHasCheckExceptionThrowingWhenBuzzIsOutOfRangeTestMethod();

            var testCaseAttributes = _checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            AssertFactorOutOfRangeTestCases(testCaseAttributes, "buzzFactor");

            var methodBody = GetMethodBodyWithoutComments(CheckExceptionThrowingWhenBuzzIsOutOfRangeMethodName);
            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);
            AssertUsesThrowsForFizzBuzzValidationException(methodBody);
        }

        [MonitoredTest("The test that checks exception throwing when Buzz is out of range should pass"), Order(10)]
        public void _10_TheTestThatChecksExceptionThrowingWhenBuzzIsOutOfRangeShouldPass()
        {
            AssertHasCheckExceptionThrowingWhenBuzzIsOutOfRangeTestMethod();
            AssertTestMethodPasses(_checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod);
        }

        [MonitoredTest("The test that checks exception throwing when Buzz is out of range should fail when in range"), Order(11)]
        public void _11_TheTestThatChecksExceptionThrowingWhenBuzzIsOutOfRangeShouldFailWhenInRange()
        {
            AssertHasCheckExceptionThrowingWhenBuzzIsOutOfRangeTestMethod();
            AssertTestMethodFails(_checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod, FizzBuzzService.MinimumFactor);
            var averageFactor = (FizzBuzzService.MinimumFactor + FizzBuzzService.MaximumFactor) / 2;
            AssertTestMethodFails(_checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod, averageFactor);
            AssertTestMethodFails(_checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod, FizzBuzzService.MaximumFactor);
        }

        [MonitoredTest("Should have a test that checks exception throwing when LastNumber is out of range"), Order(12)]
        public void _12_ShouldHaveATestThatChecksExceptionThrowingWhenLastNumberIsOutOfRange()
        {
            AssertHasCheckExceptionThrowingWhenLastNumberIsOutOfRangeTestMethod();

            var testCaseAttributes = _checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod.GetCustomAttributes()
                .OfType<TestCaseAttribute>().ToList();

            Assert.That(testCaseAttributes, Has.Count.EqualTo(2), () => "The method should have 2 test cases.");

            Assert.That(testCaseAttributes,
                Has.All.Matches((TestCaseAttribute testCase) =>
                {
                    if (testCase.Arguments[0] is int lastNumber)
                    {
                        return lastNumber < FizzBuzzService.MinimumFactor || lastNumber > FizzBuzzService.MaximumFactor;
                    }
                    return false;
                }),
                () => "All test cases must have a lastNumber that is out of range. " +
                      "Tip: The range is determined by the constants " +
                      "FizzBuzzService.MinimumLastNumber and FizzBuzzService.MaximumLastNumber");

            var methodBody = GetMethodBodyWithoutComments(CheckExceptionThrowingWhenLastNumberIsOutOfRangeMethodName);
            AssertCallsSutMethodAndUsesAssertThatSyntax(methodBody);
            AssertUsesThrowsForFizzBuzzValidationException(methodBody);
        }

        [MonitoredTest("The test that checks exception throwing when LastNumber is out of range should pass"), Order(13)]
        public void _13_TheTestThatChecksExceptionThrowingWhenLastNumberIsOutOfRangeShouldPass()
        {
            AssertHasCheckExceptionThrowingWhenLastNumberIsOutOfRangeTestMethod();
            AssertTestMethodPasses(_checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod);
        }

        [MonitoredTest("The test that checks exception throwing when LastNumber is out of range should fail when in range"), Order(14)]
        public void _14_TheTestThatChecksExceptionThrowingWhenLastNumberIsOutOfRangeShouldFailWhenInRange()
        {
            AssertHasCheckExceptionThrowingWhenLastNumberIsOutOfRangeTestMethod();
            AssertTestMethodFails(_checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod, FizzBuzzService.MinimumLastNumber);
            var avarageLastNumber = (FizzBuzzService.MinimumLastNumber + FizzBuzzService.MaximumLastNumber) / 2;
            AssertTestMethodFails(_checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod, avarageLastNumber);
            AssertTestMethodFails(_checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod, FizzBuzzService.MaximumLastNumber);
        }

        [MonitoredTest("Should have a SetUp method that runs before each test"), Order(15)]
        public void _15_ShouldHaveASetupMethodThatRunsBeforeEachTest()
        {
            //Assert
            Assert.That(_setupMethod, Is.Not.Null,
                () =>
                    "No method foud that is marked as the 'SetUp' method. " +
                    "Use the SetUp method to create a new instance of 'FizzBuzzService' before each test.");

            var regex = new Regex(@"new FizzBuzzService\(\)");
            Assert.That(regex.Matches(_testClassContent), Has.Count.EqualTo(1),
                () => "If you have a SetUp method in place, " +
                      "it is not necessary anymore to create a new instance of 'FizzBuzzService' in the test method(s). " +
                      "Use the SetUp method and a private field.");
        }

        private void AssertHasGenerateFizzBuzzWithCorrectParamtersTestMethod()
        {
            AssertHasTestMethod(
                _generateFizzBuzzWithCorrectParamtersTestMethod, GenerateFizzBuzzWithCorrectParamtersTestMethodName, 4);
        }

        private void AssertHasCheckExceptionThrowingWhenFizzIsOutOfRangeTestMethod()
        {
            AssertHasTestMethod(
                _checkExceptionThrowingWhenFizzIsOutOfRangTestMethod, CheckExceptionThrowingWhenFizzIsOutOfRangeMethodName, 1);
        }

        private void AssertHasCheckExceptionThrowingWhenBuzzIsOutOfRangeTestMethod()
        {
            AssertHasTestMethod(
                _checkExceptionThrowingWhenBuzzIsOutOfRangTestMethod, CheckExceptionThrowingWhenBuzzIsOutOfRangeMethodName, 1);
        }

        private void AssertHasCheckExceptionThrowingWhenLastNumberIsOutOfRangeTestMethod()
        {
            AssertHasTestMethod(
                _checkExceptionThrowingWhenLastNumberIsOutOfRangTestMethod, CheckExceptionThrowingWhenLastNumberIsOutOfRangeMethodName, 1);
        }

        private void AssertHasTestMethod(MethodInfo testMethod, string methodName, int expectedParameterCount)
        {
            Assert.That(testMethod, Is.Not.Null,
                () => $"Could not find test method with name '{methodName}'.");

            Assert.That(testMethod.GetParameters().Length, Is.EqualTo(expectedParameterCount),
                () => $"The method '{methodName}' should have {expectedParameterCount} parameter(s).");

            var testAttribute = testMethod.GetCustomAttributes()
                .OfType<TestAttribute>().FirstOrDefault();

            Assert.That(testAttribute, Is.Not.Null, () => "No 'Test' attribute is defined for the method.");
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
            Assert.That(() => testMethod.Invoke(_fizzBuzzTestsInstance, parameters), Throws.Nothing,
                () => $"{testMethod.Name}({StringyfyParameters(parameters)}) should pass, but doesn't.");
        }

        private void AssertTestMethodFails(MethodInfo testMethod, params object[] parameters)
        {
            Assert.That(() => testMethod.Invoke(_fizzBuzzTestsInstance, parameters), Throws.InstanceOf<Exception>(),
                () => $"{testMethod.Name}({StringyfyParameters(parameters)}) should fail, but doesn't.");
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

        private void AssertCallsSutMethodAndUsesAssertThatSyntax(string methodBody)
        {
            Assert.That(methodBody, Contains.Substring(".GenerateFizzBuzzText("),
                () => "The method should call the 'GenerateFizzBuzzText' method of a 'FizzBuzzService' instance.");
            Assert.That(methodBody, Contains.Substring("Assert.That("),
                () => "The method should use the 'Assert.That' method of NUnit.");
        }

        private void AssertUsesThrowsForFizzBuzzValidationException(string methodBody)
        {
            Assert.That(methodBody, Contains.Substring("Throws."),
                () => "The method should use the 'Throws' class of NUnit.");
            Assert.That(methodBody, Contains.Substring("FizzBuzzValidationException"),
                () => "The method should work with 'FizzBuzzValidationException'.");
        }

        private void AssertFactorOutOfRangeTestCases(IList<TestCaseAttribute> testCaseAttributes, string factorName)
        {
            Assert.That(testCaseAttributes, Has.Count.EqualTo(2), () => "The method should have 2 test cases.");

            Assert.That(testCaseAttributes,
                Has.Some.Matches((TestCaseAttribute testCase) =>
                {
                    if (testCase.Arguments[0] is int factor)
                    {
                        return factor < FizzBuzzService.MinimumFactor;
                    }
                    return false;
                }),
                () => $"At least one test case must have a {factorName} that is smaller than the minimum. " +
                      "Tip: The range is determined by the constants " +
                      "FizzBuzzService.MinimumFactor and FizzBuzzService.MaximumFactor");

            Assert.That(testCaseAttributes,
                Has.Some.Matches((TestCaseAttribute testCase) =>
                {
                    if (testCase.Arguments[0] is int fizz)
                    {
                        return fizz > FizzBuzzService.MaximumFactor;
                    }
                    return false;
                }),
                () => $"At least one test cases must have a {factorName} that is greather than the maximum. " +
                      "Tip: The range is determined by the constants " +
                      "FizzBuzzService.MinimumFactor and FizzBuzzService.MaximumFactor");
        }
    }
}
