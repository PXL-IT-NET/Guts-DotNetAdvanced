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
    [ExerciseTestFixture("dotnet2", "H11", "Exercise01", 
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
            Assert.That(hash, Is.EqualTo("78-F5-05-82-E7-D7-09-7F-B0-D6-30-DA-05-27-7F-E6"), () => "'1_SelectExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\2_WhereExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("11-E5-9C-E9-9F-DB-43-36-90-B6-8A-6E-4A-6D-92-7C"), () => "'2_WhereExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\3_OrderByExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("5D-45-C9-5B-51-FC-DB-6D-62-05-16-D9-93-B9-D3-F3"), () => "'3_OrderByExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\4_GroupExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("72-A0-4E-9D-B1-51-F4-48-8E-08-1E-AA-6E-F0-14-F9"), () => "'4_GroupExamplesTests.cs' has been changed.");

            hash = Solution.Current.GetFileHash(@"LinqExamples.Tests\5_JoinExamplesTests.cs");
            Assert.That(hash, Is.EqualTo("00-F1-5C-B6-5D-4D-85-55-54-7B-E5-23-55-97-27-A8"), () => "'5_JoindExamplesTests.cs' has been changed.");
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
            content = CodeCleaner.StripComments(content).Replace(" ",  "").ToLower();

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
