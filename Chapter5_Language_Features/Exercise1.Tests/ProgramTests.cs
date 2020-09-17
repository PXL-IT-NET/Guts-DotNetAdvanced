using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Exercise1.Tests
{
    [ExerciseTestFixture("dotnet2", "H5", "Exercise01", @"Exercise1\Program.cs")]
    public class ProgramTests
    {
        private string _programClassContent;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _programClassContent = Solution.Current.GetFileContent(@"Exercise1\Program.cs");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            var consoleReader = new StringReader(" ");
            Console.SetIn(consoleReader);
        }

        [MonitoredTest("Program - Length of the code should be short")]
        public void _01_LengthOfTheCode_ShouldBeShort()
        {
            int maximumCharacterCount = 700;
            Assert.That(_programClassContent.Length, Is.LessThanOrEqualTo(maximumCharacterCount),
                $"The main method is too long. It could be done in {maximumCharacterCount} or less.");
        }

        [MonitoredTest("Program - Main - Should run BalloonProgram")]
        public void _02_Main_ShouldRunBalloonProgram()
        {
            string mainBody = GetMethodBodyWithoutComments(nameof(Program.Main));
            Assert.That(mainBody, Contains.Substring("new BalloonProgram"), "An instance of BalloonProgram should be created.");
            Assert.That(mainBody, Contains.Substring(".Run();"), "The 'Run' method of the balloon program should be called.");
        }

        [MonitoredTest("Program - Main - Should write to debug output and console")]
        public void _03_Main_ShouldWriteToDebugAndConsole()
        {
            //Listen to console
            var consoleReader = new StringReader(" ");
            var consoleWriter = new StringWriter();
            Console.SetOut(consoleWriter);
            Console.SetIn(consoleReader);

            //Listen to debug
            var debugWriter = new StringWriter();
            var debugListener = new TextWriterTraceListener(debugWriter);
            Debug.Listeners.Add(debugListener);

            //execute the program
            Program.Main(null);

            string consoleOutput = consoleWriter.ToString();
            string debugOutput = debugWriter.ToString();

            Assert.That(consoleOutput, Is.Not.Empty, "Nothing has been written to the console.");
            Assert.That(debugOutput, Is.Not.Empty, "Nothing has been written to the debug window.");

            string[] consoleLines = consoleOutput.Split(new string[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(consoleLines.Count(line => line.ToLower().Contains("balloon")), Is.GreaterThanOrEqualTo(5),
                "At least 5 lines, written to the console, should contain the text 'balloon'.");

            Assert.That(consoleOutput.ToUpper().Contains(debugOutput), Is.True,
                "The debug output should be the same as the console output with all text upper case.");
            // Assert.That(CallsMemberMethod("NextBalloon"), Is.True, "Cannot find an invocation of the 'NextBalloon' method of a 'Random' instance.");
        }

        private string GetMethodBodyWithoutComments(string methodName)
        {
            var syntaxtTree = CSharpSyntaxTree.ParseText(_programClassContent);
            var root = syntaxtTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals(methodName));
            Assert.That(method, Is.Not.Null,
                () => $"Could not find the '{methodName}' method. You may have accidentally deleted or renamed it?");

            //filter out comments
            IEnumerable<StatementSyntax> realStatements = method.Body.Statements.Where(s =>
                s.Kind() != SyntaxKind.SingleLineCommentTrivia && s.Kind() != SyntaxKind.MultiLineCommentTrivia);

            var builder = new StringBuilder();
            foreach (StatementSyntax statement in realStatements)
            {
                builder.AppendLine(statement.ToString());
            }
            return builder.ToString();
        }

    }
}