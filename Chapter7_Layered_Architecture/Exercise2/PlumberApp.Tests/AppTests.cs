using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using PlumberApp.Infrastructure.Storage;
using PlumberApp.UI;

namespace PlumberApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise02", @"PlumberApp.UI\App.xaml.cs")]
    public class AppTests
    {
        private string _appClassContent;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _appClassContent = Solution.Current.GetFileContent(@"PlumberApp.UI\App.xaml.cs");
        }

        [MonitoredTest("OnStartup - Should inject a WorkloadFileRepository instance into the MainWindow and show it")]
        public void OnStartup_ShouldInjectAWorkloadFileRepositoryInstanceIntoTheMainWindowAndShowIt()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_appClassContent);
            var root = syntaxTree.GetRoot();
            MethodDeclarationSyntax onStartupMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("OnStartup"));

            Assert.That(onStartupMethod, Is.Not.Null, "Cannot find a method 'OnStartup' in App.xaml.cs");

            List<ObjectCreationExpressionSyntax> objectCreations = onStartupMethod.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().ToList();

            ObjectCreationExpressionSyntax workloadFileRepositoryCreation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(WorkloadFileRepository));
            Assert.That(workloadFileRepositoryCreation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of WorkloadFileRepository is created.");

            ObjectCreationExpressionSyntax mainWindowCreation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(MainWindow));
            Assert.That(mainWindowCreation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of MainWindow is created.");

            var bodyBuilder = new StringBuilder(); //no pun intended :)
            foreach (var statement in onStartupMethod.Body.Statements)
            {
                bodyBuilder.AppendLine(statement.ToString());
            }
            string body = bodyBuilder.ToString();

            Assert.That(body, Contains.Substring("Environment.GetFolderPath"),
                "The folder to save workloads in, should be in the special 'AppData' directory. Use the 'Environment' class to retrieve the path of that directory.");

            Assert.That(body, Contains.Substring("Environment.SpecialFolder.ApplicationData"),
                "The folder to save workloads in, should be in the special 'AppData' directory. Use the 'Environment.SpecialFolder' enum.");

            Assert.That(body, Contains.Substring("Path.Combine(").And.Contains(@"""PlumberApp"")"),
                "The folder to save workloads in, should be a subdirectory 'PlumberApp' in the special 'AppData' directory. " +
                "Use the static 'Combine' method of the 'System.IO.Path' class to create a string that holds the complete directory path.");

            Assert.That(body, Contains.Substring(".Show();"),
                "The MainWindow is instantiated, but not shown in the 'OnStartup' method.");
        }
    }
}