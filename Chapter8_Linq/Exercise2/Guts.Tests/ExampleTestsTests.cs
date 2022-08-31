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
    [ExerciseTestFixture("dotnet2", "H08", "Exercise02",
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
            Assert.That(hash, Is.EqualTo("CB-4C-36-EB-C6-61-F5-40-32-81-43-5F-7A-86-28-C3"), "'1_SelectExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\2_WhereExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("31-B0-EF-11-62-DA-BA-C3-36-C6-10-2B-5C-13-34-6C"), "'2_WhereExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\3_OrderByExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("80-6A-76-57-21-60-F4-03-22-88-E1-AD-86-48-53-61"), "'3_OrderByExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\4_GroupExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("AE-C4-CD-6B-1B-43-E3-5C-C1-67-81-C4-EF-3F-4F-EA"), "'4_GroupExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\5_JoinExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("25-5C-06-34-E4-EA-DC-17-D4-F1-AC-32-9B-6E-9C-23"), "'5_JoinExamplesTests.cs' has been changed.");
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