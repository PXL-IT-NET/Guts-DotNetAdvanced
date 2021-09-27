using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Exercise3.OrderAggregate;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Exercise3.Tests
{
    [TestFixture]
    //[ExerciseTestFixture("dotnet2", "H5", "Exercise03", @"Exercise3\OrderAggregate\Order.cs")]
    public class OrderTests
    {
        private static readonly Random Random = new Random();
        private string _orderClassContent;
        private List<string> _propertyChanges;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _orderClassContent = Solution.Current.GetFileContent(@"Exercise3\OrderAggregate\Order.cs");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _propertyChanges = new List<string>();
        }

        [MonitoredTest("Order - Should implement IOrder"), Order(1)]
        public void _01_ShouldImplementIOrder()
        {
            AssertImplementsIOrder();
        }

        [MonitoredTest("Order - Constructor - Should initialize properly"), Order(2)]
        public void _02_Constructor_ShouldInitializeProperly()
        {
            AssertImplementsIOrder();

            //Arrange
            int numberOfBurgers = Random.Next(1, 11);

            //Act
            IOrder order = new Order(numberOfBurgers) as IOrder;

            //Assert
            Assert.That(order.NumberOfBurgers, Is.EqualTo(numberOfBurgers),
                "'NumberOfBurgers' is not correctly initialized.");
            Assert.That(order.IsStarted, Is.False,
                "'IsStarted' is not correctly initialized.");
            Assert.That(order.IsCompleted, Is.False,
                "'IsCompleted' is not correctly initialized.");

            string orderNumber = order.Number.Equals(default(OrderNumber)) ? string.Empty : order.Number.ToString();
            Assert.That(orderNumber.Length, Is.GreaterThanOrEqualTo(2),
                "'Number' is not correctly initialized.");
            Assert.That(orderNumber[0], Is.EqualTo('#'),
                "'Number' is not correctly initialized.");
            Assert.That(char.IsDigit(orderNumber[1]), Is.True,
                "'Number' is not correctly initialized.");

            string constructorBody = GetConstructorBodyWithoutComments();
            Assert.That(constructorBody, Contains.Substring("OrderNumber.CreateNext()").IgnoreCase,
                "The static 'CreateNext' method of 'OrderNumber' should have been called to determine the 'Number' of the order.");
        }

        [MonitoredTest("Order - Should implement INotifyPropertyChanged"), Order(3)]
        public void _03_ShouldImplementINotifyPropertyChanged()
        {
            AssertImplementsINotifyPropertyChanged();
        }

        [MonitoredTest("Order - IsStarted - Should notify when set"), Order(4)]
        public void _04_IsStarted_ShouldNotifyWhenSet()
        {
            AssertImplementsIOrder();
            AssertImplementsINotifyPropertyChanged();

            //Arrange
            Order order = new Order(1);

            IOrder orderAsInterface = order as IOrder;
            INotifyPropertyChanged orderAsNotifier = order as INotifyPropertyChanged;
            orderAsNotifier.PropertyChanged += OnOrderPropertyChanged;

            //Act
            orderAsInterface.IsStarted = true;

            //Assert
            Assert.That(_propertyChanges, Has.One.EqualTo("IsStarted"));
        }

        [MonitoredTest("Order - IsCompleted - Should notify when set"), Order(5)]
        public void _05_IsCompleted_ShouldNotifyWhenSet()
        {
            AssertImplementsIOrder();
            AssertImplementsINotifyPropertyChanged();

            //Arrange
            Order order = new Order(1);

            IOrder orderAsInterface = order as IOrder;
            INotifyPropertyChanged orderAsNotifier = order as INotifyPropertyChanged;
            orderAsNotifier.PropertyChanged += OnOrderPropertyChanged;

            //Act
            orderAsInterface.IsCompleted = true;

            //Assert
            Assert.That(_propertyChanges, Has.One.EqualTo("IsCompleted"));
        }

        private void OnOrderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _propertyChanges.Add(e.PropertyName);
        }

        private void AssertImplementsIOrder()
        {
            Order order = new Order(1);
            IOrder abstractOrder = order as IOrder;
            Assert.That(abstractOrder, Is.Not.Null, "The 'Order' class should implement the 'IOrder' interface.");
        }

        private string GetConstructorBodyWithoutComments()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_orderClassContent);
            var root = syntaxTree.GetRoot();
            var constructors = root
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .ToList();
            Assert.That(constructors.Count, Is.EqualTo(1), $"The 'Order' class should have exactly 1 constructor. {constructors.Count} constructors are found.");
            var constructor = constructors.First();

            //filter out comments
            IEnumerable<StatementSyntax> realStatements = constructor.Body.Statements.Where(s =>
                s.Kind() != SyntaxKind.SingleLineCommentTrivia && s.Kind() != SyntaxKind.MultiLineCommentTrivia);

            var builder = new StringBuilder();
            foreach (StatementSyntax statement in realStatements)
            {
                builder.AppendLine(statement.ToString());
            }
            return builder.ToString();
        }

        private void AssertImplementsINotifyPropertyChanged()
        {
            Order order = new Order(1);
            INotifyPropertyChanged notifier = order as INotifyPropertyChanged;
            Assert.That(notifier, Is.Not.Null, "The 'Order' class should implement the 'INotifyPropertyChanged' interface.");
        }
    }
}