using Guts.Client.Shared;
using LinqExamples.Tests;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using Guts.Client.Classic;
using Guts.Client.Shared.TestTools;

namespace Guts.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02",
        @"LinqExamples\SelectExamples.cs;
LinqExamples\WhereExamples.cs;
LinqExamples\OrderByExamples.cs;
LinqExamples\GroupExamples.cs;
LinqExamples\JoinExamples.cs;")]
    public class ExampleTestsTests
    {
        [MonitoredTest("Should not have changed test files"), Order(1)]
        public void _01_ShouldNotHaveChangedTestFiles()
        {
            var hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\1_SelectExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("77-D8-A5-61-80-8D-EA-0B-6B-8A-39-F9-F7-27-3B-85"), () => "'1_SelectExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\2_WhereExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("9D-7E-52-9B-EA-09-39-F4-F0-23-5D-3B-A4-0A-91-9D"), () => "'2_WhereExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\3_OrderByExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("D6-D1-9C-2D-4F-81-4F-F0-6D-37-06-1A-4A-14-17-70"), () => "'3_OrderByExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\4_GroupExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("35-30-53-49-3A-21-C2-D0-9D-B5-7E-4B-36-EE-6F-67"), () => "'4_GroupExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\5_JoinExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("B3-B3-21-62-7D-5C-D1-7F-F3-EB-07-3C-7E-CA-FF-AD"), () => "'5_JoinExamplesTests.cs' has been changed.");
        }

        [MonitoredTest("Should use LINQ"), Order(2)]
        public void _02_ShouldUseLinq()
        {
            AssertUsesLinq("SelectExamples.cs");
            AssertUsesLinq("WhereExamples.cs");
            AssertUsesLinq("OrderByExamples.cs");
            AssertUsesLinq("GroupExamples.cs");
            AssertUsesLinq("JoinExamples.cs");
        }

        [MonitoredTest("All select example tests should pass"), Order(3)]
        public void _03_SelectExampleTests_ShouldAllPass()
        {
            var testClassInstance = new SelectExamplesTests();
            AssertAllTestMethodsPass(testClassInstance);
        }

        [MonitoredTest("All where example tests should pass"), Order(4)]
        public void _04_WhereExampleTests_ShouldAllPass()
        {
            var testClassInstance = new WhereExamplesTests();
            AssertAllTestMethodsPass(testClassInstance);
        }

        [MonitoredTest("All orderby example tests should pass"), Order(5)]
        public void _05_OrderByExampleTests_ShouldAllPass()
        {
            var testClassInstance = new OrderByExamplesTests();
            AssertAllTestMethodsPass(testClassInstance);
        }

        [MonitoredTest("All group example tests should pass"), Order(6)]
        public void _06_GroupExampleTests_ShouldAllPass()
        {
            var testClassInstance = new GroupExamplesTests();
            AssertAllTestMethodsPass(testClassInstance);
        }

        [MonitoredTest("All join example tests should pass"), Order(7)]
        public void _07_JoinExampleTests_ShouldAllPass()
        {
            var testClassInstance = new JoinExamplesTests();
            AssertAllTestMethodsPass(testClassInstance);
        }

        private static void AssertUsesLinq(string sourceFileName)
        {
            var content = Solution.Current.GetFileContent($@"LinqExamples\{sourceFileName}").Trim();
            content = CodeCleaner.StripComments(content).Replace(" ", "").ToLower();

            Assert.That(content, Does.Not.Contain("for("), () => $"A for-loop is used in '{sourceFileName}'. " +
                                                                 "This is not necessary when LINQ is used.");

            Assert.That(content, Does.Not.Contain("foreach("), () => $"A foreach-loop is used in '{sourceFileName}'. " +
                                                                     "This is not necessary when LINQ is used.");

            Assert.That(content, Does.Not.Contain("while("), () => $"A while-loop is used in '{sourceFileName}'. " +
                                                                   "This is not necessary when LINQ is used.");
        }

        private void AssertAllTestMethodsPass(Object testClassInstance)
        {
            var testClassType = testClassInstance.GetType();

            var setupMethod = testClassType.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<SetUpAttribute>() != null);

            var testMethodInfos = testClassType.GetMethods().Where(m => m.GetCustomAttribute<TestAttribute>() != null)
                .ToList();

            foreach (var testMethodInfo in testMethodInfos)
            {
                if (setupMethod != null)
                {
                    setupMethod.Invoke(testClassInstance, new object[0]);
                }
                AssertTestMethodPasses(testClassInstance, testMethodInfo);
            }
        }

        private void AssertTestMethodPasses(object testClassInstance, MethodInfo testMethod)
        {
            Assert.That(() => testMethod.Invoke(testClassInstance, new object[0]), Throws.Nothing,
                () => $"{testMethod.Name}() should pass, but doesn't.");
        }
    }
}
