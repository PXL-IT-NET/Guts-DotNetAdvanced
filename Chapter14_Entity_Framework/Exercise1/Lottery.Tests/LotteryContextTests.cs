using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Lottery.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata;
using NUnit.Framework;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise01", @"Lottery.Data\LotteryContext.cs;Lottery.Data\LotteryGameRepository.cs;Lottery.Data\DrawRepository.cs;Lottery.Business\DrawService.cs;Lottery.UI\LotteryWindow.xaml;Lottery.UI\LotteryWindow.xaml.cs;Lottery.UI\Converters\DrawNumbersConverter.cs;")]
    public class LotteryContextTests : DatabaseTests
    {
        private string _lotterContextClassContent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _lotterContextClassContent = Solution.Current.GetFileContent(@"Lottery.Data\LotteryContext.cs");
        }

        [MonitoredTest("LotteryContext - Should have two DBSets")]
        public void ShouldHaveTwoDbSets()
        {
            var properties = GetDbSetProperties();

            Assert.That(properties, Has.Count.EqualTo(2), () => "There should be exactly 2 'DbSet' properties.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<LotteryGame>"),
                () => "Ther should be one 'DbSet' for lottery games.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<Draw>"),
                () => "Ther should be one 'DbSet' for draws.");
        }

        [MonitoredTest("LotteryContext - OnModelCreating should seed at least 2 games")]
        public void OnModelCreating_ShouldSeedAtLeast2Games()
        {
            using (var context = CreateDbContext())
            {
                var amountOfSeededGames = context.Set<LotteryGame>().Count();
                Assert.That(amountOfSeededGames, Is.GreaterThanOrEqualTo(2), () => "The database must be seeded wit at least 2 lottery games. " +
                                                                                   $"Now the database contains {amountOfSeededGames} games after creation.");
            }
        }

        [MonitoredTest("LotteryContext - OnModelCreating should make the name of a lottery game required")]
        public void OnModelCreating_ShouldMakeTheNameOfALotteryGameRequired()
        {
            using (var context = CreateDbContext())
            {
                var gameEntityType = context.Model.FindEntityType(typeof(LotteryGame).FullName);
                var nameProperty = gameEntityType.FindProperty("Name");
                Assert.That(nameProperty.IsNullable, Is.False, () => "The 'Name' property is not required. " +
                                                                     "Use Fluent API to tell Entity Framework that the 'Name' property of a 'LotteryGame' enity should be required.");
            }
        }

        [MonitoredTest("LotteryContext - OnModelCreating should define the composite primary key for the DrawNumber entity")]
        public void OnModelCreating_ShouldDefineTheCompositePrimaryKeyForTheDrawNumberEntity()
        {
            using (var context = CreateDbContext())
            {
                var drawNumberEntityType = context.Model.FindEntityType(typeof(DrawNumber).FullName);
                var primaryKey = drawNumberEntityType.FindPrimaryKey();

                Assert.That(primaryKey.Properties, Has.One.Matches((IProperty p) => p.Name == "DrawId"));
                Assert.That(primaryKey.Properties, Has.One.Matches((IProperty p) => p.Name == "Number"));
            }
        }

        [MonitoredTest("LotteryContext - OnConfiguring should use the app.config connection string to configure a SQL database connection")]
        public void OnConfiguring_ShouldUseTheAppConfigConnectionStringToConfigureASqlDatabaseConnection()
        {
            var methodBody = GetMethodBody("OnConfiguring");

            Assert.That(methodBody.Statements.Count == 1 && methodBody.Statements[0] is IfStatementSyntax, Is.True,
                () => "The method body should only have one if-statement. " +
                      "The body of the if-statement should contain the code to configure the database.");

            var ifStatement = (IfStatementSyntax)methodBody.Statements[0];
            Assert.That(ifStatement.Else, Is.Null, () => "The if-statement does not need to have an 'else'.");
            var ifStatementBody = ifStatement.ToString();

            Assert.That(ifStatementBody,
                Contains.Substring("ConfigurationManager.ConnectionStrings[\"LotteryConnectionString\"].ConnectionString"),
                () => "The connectionstring should be correctly retrieved from the app.config.");

            Assert.That(ifStatementBody, Contains.Substring("optionsBuilder.UseSqlServer("),
                () => "You should tell Entity Framework that is should use the SQL Server provider.");
        }

        [MonitoredTest("LotteryContext - CreateOrUpdateDatabase should trigger a database migration")]
        public void CreateOrUpdateDatabase_ShouldTriggerADatabaseMigration()
        {
            var methodBody = GetMethodBody("CreateOrUpdateDatabase").ToString();

            Assert.That(methodBody, Contains.Substring("Database.Migrate();"), () => "You should use the 'Migrate' method of the 'Database' property of the context (this) to migrate the database. " +
                                                                                     "If it does not exist yet, it is created. " +
                                                                                     "Otherwise the database is migrated to the latest version.");
        }

        [MonitoredTest("LotteryContext - Should not have unnecessary comments")]
        public void ShouldNotHaveUnnecessaryComments()
        {
            var syntaxtTree = CSharpSyntaxTree.ParseText(_lotterContextClassContent);
            var root = syntaxtTree.GetRoot();
            var commentCount = root
                .DescendantTrivia()
                .Count(trivia => trivia.Kind() == SyntaxKind.SingleLineCommentTrivia ||
                    trivia.Kind() == SyntaxKind.MultiLineCommentTrivia);

            Assert.That(commentCount, Is.LessThanOrEqualTo(4), () => "Clean up code that is commented out " +
                                                                     "and/or replace comments with meaningful method calls.");
        }

        private BlockSyntax GetMethodBody(string methodName)
        {
            var syntaxtTree = CSharpSyntaxTree.ParseText(_lotterContextClassContent);
            var root = syntaxtTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals(methodName));
            Assert.That(method, Is.Not.Null,
                () => $"Could not find the '{methodName}' method. You may have accidentially deleted or renamed it?");
            return method.Body;
        }

        private IList<PropertyDeclarationSyntax> GetDbSetProperties()
        {
            var syntaxtTree = CSharpSyntaxTree.ParseText(_lotterContextClassContent);
            var root = syntaxtTree.GetRoot();
            var properties = root
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p =>
                {
                    if (!(p.Type is GenericNameSyntax genericName)) return false;
                    return genericName.Identifier.ValueText == "DbSet";
                });

            return properties.ToList();
        }
    }
}