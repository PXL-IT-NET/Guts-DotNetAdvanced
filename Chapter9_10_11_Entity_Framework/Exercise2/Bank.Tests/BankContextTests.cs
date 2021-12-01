using System.Collections.Generic;
using System.Linq;
using Bank.Domain;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H11", "Exercise02", @"Bank.Infrastructure\BankContext.cs")]
    public class BankContextTests : DatabaseTests
    {
        private string _bankContextClassContent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bankContextClassContent = Solution.Current.GetFileContent(@"Bank.Infrastructure\BankContext.cs");
        }

        [MonitoredTest("BankContext - Should have 3 DBSets")]
        public void ShouldHave3DbSets()
        {
            var properties = GetDbSetProperties();

            Assert.That(properties, Has.Count.EqualTo(3), "There should be exactly 3 'DbSet' properties.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<Customer>"),
                "There should be one 'DbSet' for customers.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<Account>"),
                "There should be one 'DbSet' for accounts.");
            Assert.That(properties,
                Has.One.Matches((PropertyDeclarationSyntax p) => p.Type.ToString() == "DbSet<City>"),
                "There should be one 'DbSet' for cities.");
        }

        [MonitoredTest("BankContext - OnModelCreating should make the name and firstname of a customer required")]
        public void OnModelCreating_ShouldMakeTheNameAndFirstNameOfACustomerRequired()
        {
            using (var context = CreateDbContext())
            {
                var customerEntityType = context.Model.FindEntityType(typeof(Customer).FullName);
                var nameProperty = customerEntityType.FindProperty(nameof(Customer.Name));
                Assert.That(nameProperty.IsNullable, Is.False, "The 'Name' property is not required. " +
                                                               "Use Fluent API to tell Entity Framework that the 'Name' property of a 'Customer' entity should be required.");
                var firstNameProperty = customerEntityType.FindProperty(nameof(Customer.FirstName));
                Assert.That(firstNameProperty.IsNullable, Is.False, "The 'FirstName' property is not required. " +
                                                                    "Use Fluent API to tell Entity Framework that the 'FirstName' property of a 'Customer' entity should be required.");
            }
        }

        [MonitoredTest("BankContext - OnModelCreating should set the zipcode as primary key for the City entity")]
        public void OnModelCreating_ShouldSetTheZipCodeAsPrimaryKeyForTheCityEntity()
        {
            using (var context = CreateDbContext())
            {
                var cityEntityType = context.Model.FindEntityType(typeof(City).FullName);
                var primaryKey = cityEntityType.FindPrimaryKey();

                Assert.That(primaryKey.Properties.Count, Is.EqualTo(1),
                    $"Expected one primary key property, but the primary key consists out of {primaryKey.Properties.Count} properties.");
                var primaryKeyProperty = primaryKey.Properties.First();
                Assert.That(primaryKeyProperty.Name, Is.EqualTo("zipcode").IgnoreCase,
                    $"The primary key property is '{primaryKeyProperty.Name}' instead of 'ZipCode'.");
            }
        }

        [MonitoredTest("BankContext - OnModelCreating should seed the flemish capital cities")]
        public void OnModelCreating_ShouldSeedTheFlemishCapitalCities()
        {
            using (var context = CreateDbContext())
            {
                var seededCities = context.Set<City>().ToList();
                Assert.That(seededCities, Has.Count.GreaterThanOrEqualTo(5), "The database must be seeded wit at least 5 cities. " +
                                                                             $"Now the database contains {seededCities.Count} cities after creation.");
                AssertCity(seededCities, "Antwerpen", 2000);
                AssertCity(seededCities, "Brugge", 8000);
                AssertCity(seededCities, "Gent", 9000);
                AssertCity(seededCities, "Hasselt", 3500);
                AssertCity(seededCities, "Leuven", 3000);
            }
        }

        [MonitoredTest("BankContext - OnModelCreating should mark the number of an account as primary key")]
        public void OnModelCreating_ShouldMarkTheNumberOfAnAccountAsPrimaryKey()
        {
            using (var context = CreateDbContext())
            {
                var accountEntityType = context.Model.FindEntityType(typeof(Account).FullName);
                var accountNumberProperty = accountEntityType.FindProperty(nameof(Account.AccountNumber));

                Assert.That(accountNumberProperty.IsPrimaryKey(), Is.True, "The 'AccountNumber' property is not the primary key. " +
                                                                            "Use Fluent API to tell Entity Framework that the 'AccountNumber' property of an 'Account' entity should be the primary key.");
            }
        }

        [MonitoredTest("BankContext - OnModelCreating should configure the relation between Customer and City correctly")]
        public void OnModelCreating_ShouldConfigureTheRelationBetweenCustomerAndCity()
        {
            using (var context = CreateDbContext())
            {
                var customerEntityType = context.Model.FindEntityType(typeof(Customer).FullName);
                var cityForeignKey = customerEntityType.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(City));
                Assert.That(cityForeignKey, Is.Not.Null,
                    "No foreign key relation found between Customer and City. The 'ZipCode' of the customer should point to a primary key of City.");
            }
        }

        [MonitoredTest("BankContext - CreateOrUpdateDatabase should trigger a database migration")]
        public void CreateOrUpdateDatabase_ShouldTriggerADatabaseMigration()
        {
            var methodBody = GetMethodBody("CreateOrUpdateDatabase").ToString();

            Assert.That(methodBody, Contains.Substring("Database.Migrate();"), () => "You should use the 'Migrate' method of the 'Database' property of the context (this) to migrate the database. " + "Otherwise the database is migrated to the latest version.");
        }

        [MonitoredTest("BankContext - Should not have unnecessary comments")]
        public void ShouldNotHaveUnnecessaryComments()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_bankContextClassContent);
            var root = syntaxTree.GetRoot();
            var commentCount = root
                .DescendantTrivia()
                .Count(trivia => trivia.Kind() == SyntaxKind.SingleLineCommentTrivia ||
                                 trivia.Kind() == SyntaxKind.MultiLineCommentTrivia);

            Assert.That(commentCount, Is.LessThanOrEqualTo(4), () => "Clean up code that is commented out " +
                                                                     "and/or replace comments with meaningful method calls.");
        }

        private BlockSyntax GetMethodBody(string methodName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_bankContextClassContent);
            var root = syntaxTree.GetRoot();
            var method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals(methodName));
            Assert.That(method, Is.Not.Null,
                () => $"Could not find the '{methodName}' method. You may have accidentally deleted or renamed it?");
            return method.Body;
        }

        private IList<PropertyDeclarationSyntax> GetDbSetProperties()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_bankContextClassContent);
            var root = syntaxTree.GetRoot();
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

        private void AssertCity(IList<City> cities, string name, int zipCode)
        {
            Assert.That(cities.Any(c => c.Name == name && c.ZipCode == zipCode), Is.True,
                $"City with name '{name}' && zip '{zipCode}' is not seeded.");
        }
    }
}