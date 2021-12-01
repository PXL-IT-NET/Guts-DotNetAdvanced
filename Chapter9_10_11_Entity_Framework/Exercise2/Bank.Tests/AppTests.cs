using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank.AppLogic;
using Bank.Infrastructure;
using Bank.UI;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.UI\App.xaml;Bank.UI\App.xaml.cs")]
    public class AppTests
    {
        private string _appClassContent;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _appClassContent = Solution.Current.GetFileContent(@"Bank.UI\App.xaml.cs");
        }

        [MonitoredTest("OnStartup - Should wire repositories and services together and inject them in the CustomersWindow")]
        public void OnStartup_ShouldInjectAWorkloadFileRepositoryInstanceIntoTheMainWindowAndShowIt()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_appClassContent);
            var root = syntaxTree.GetRoot();
            MethodDeclarationSyntax onStartupMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("OnStartup"));

            Assert.That(onStartupMethod, Is.Not.Null, "Cannot find a method 'OnStartup' in App.xaml.cs");

            var bodyBuilder = new StringBuilder(); //no pun intended :)
            foreach (var statement in onStartupMethod.Body.Statements)
            {
                bodyBuilder.AppendLine(statement.ToString());
            }
            string body = bodyBuilder.ToString();

            List<ObjectCreationExpressionSyntax> objectCreations = onStartupMethod.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().ToList();

            ObjectCreationExpressionSyntax creation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(BankContext));
            Assert.That(creation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of 'BankContext' is created.");

            Assert.That(body, Contains.Substring(".CreateOrUpdateDatabase();"),
                "The 'CreateOrUpdateDatabase' method should be called on the 'BankContext' instance.");

            creation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(CityRepository));
            Assert.That(creation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of 'CityRepository' is created.");

            creation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(CustomerRepository));
            Assert.That(creation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of 'CustomerRepository' is created.");

            creation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(AccountRepository));
            Assert.That(creation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of 'AccountRepository' is created.");

            creation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(AccountService));
            Assert.That(creation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of 'AccountService' is created.");

            creation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(WindowDialogService));
            Assert.That(creation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of 'WindowDialogService' is created.");

            creation = objectCreations.FirstOrDefault(oc => oc.Type.ToString() == nameof(CustomersWindow));
            Assert.That(creation, Is.Not.Null, "Cannot find a statement in 'OnStartup' where a new instance of CustomersWindow is created.");

            Assert.That(body, Contains.Substring(".Show();"),
                "The CustomersWindow is instantiated, but not shown in the 'OnStartup' method.");
        }
    }
}