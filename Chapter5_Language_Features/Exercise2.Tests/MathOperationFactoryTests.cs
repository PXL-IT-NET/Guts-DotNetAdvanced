using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Exercise2.Tests
{
    [ExerciseTestFixture("dotnet2", "H05", "Exercise02", @"Exercise2\MathOperationFactory.cs")]
    public class MathOperationFactoryTests
    {
        private MathOperationFactory _factory;
        private string _factoryClassContent;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _factoryClassContent = Solution.Current.GetFileContent(@"Exercise2\MathOperationFactory.cs");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _factory = new MathOperationFactory();
        }

        [MonitoredTest("MathOperationFactory - CreateCubicOperation - Should use a lambda expression")]
        public void CreateCubicOperation_ShouldUseALambdaExpression()
        {
            SyntaxNode body = GetMethodBody(nameof(MathOperationFactory.CreateCubicOperation));
            var lambdaSyntax = body.DescendantNodes().OfType<LambdaExpressionSyntax>().FirstOrDefault();
            Assert.That(lambdaSyntax, Is.Not.Null);
        }

        [MonitoredTest("MathOperationFactory - CreateCubicOperation - Should return the correct function")]
        public void CreateCubicOperation_ShouldReturnTheCorrectFunction()
        {
            //Arrange
            int[] inputs = { 1, 2, 5, 10, 100, 1000 };
            long[] expectedOutputs = { 6, 34, 430, 3210, 3020100, 3002001000 };

            //Act
            var operation = _factory.CreateCubicOperation();

            //Assert
            for (int i = 0; i < inputs.Length; i++)
            {
                long result = operation(inputs[i]);
                Assert.That(result, Is.EqualTo(expectedOutputs[i]),
                    $"3x³ + 2x² + x should be {expectedOutputs[i]} when x is {inputs[i]}.");
            }
        }

        [MonitoredTest("MathOperationFactory - CreateNthPrimeOperation - Should use a lambda expression")]
        public void CreateNthPrimeOperation_ShouldUseALambdaExpression()
        {
            SyntaxNode body = GetMethodBody(nameof(MathOperationFactory.CreateNthPrimeOperation));
            var lambdaSyntax = body.DescendantNodes().OfType<LambdaExpressionSyntax>().FirstOrDefault();
            Assert.That(lambdaSyntax, Is.Not.Null);
        }

        [MonitoredTest("MathOperationFactory - CreateNthPrimeOperation - Should return the correct function")]
        public void CreateNthPrimeOperation_ShouldReturnTheCorrectFunction()
        {
            //Arrange
            int[] inputs = { 1, 2, 4, 5, 10, 100 };
            long[] expectedOutputs = { 2, 3, 7, 11, 29, 541 };

            //Act
            var operation = _factory.CreateNthPrimeOperation();

            //Assert
            for (int i = 0; i < inputs.Length; i++)
            {
                long result = operation(inputs[i]);
                Assert.That(result, Is.EqualTo(expectedOutputs[i]),
                    $"The {inputs[i]}th prime number should be {expectedOutputs[i]}.");
            }
        }

        private SyntaxNode GetMethodBody(string methodName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_factoryClassContent);
            var root = syntaxTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals(methodName));
            Assert.That(method, Is.Not.Null,
                () => $"Could not find the '{methodName}' method. You may have accidentally deleted or renamed it?");

            if (method.Body != null) return method.Body;
            if (method.ExpressionBody != null) return method.ExpressionBody;
            Assert.Fail($"Could not find the body (or expression body) of the '{methodName}' method.");
            return null;
        }
    }
}