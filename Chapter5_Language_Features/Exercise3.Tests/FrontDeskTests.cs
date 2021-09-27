using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Exercise3.FrontDeskAggregate;
using Exercise3.OrderAggregate;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Exercise3.Tests
{
    [TestFixture]
    //[ExerciseTestFixture("dotnet2", "H5", "Exercise03", @"Exercise3\FrontDeskAggregate\FrontDesk.cs")]
    public class FrontDeskTests
    {
        private static readonly Random Random = new Random();

        private Type _frontDeskType;
        private PropertyInfo _ordersProperty;
        private FieldInfo _ordersBackingField;
        private FrontDesk _frontDesk;
        private EventInfo _eventInfo;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            //  _orderClassContent = Solution.Current.GetFileContent(@"Exercise3\OrderAggregate\Order.cs");
            _frontDeskType = typeof(FrontDesk);
            _ordersProperty = _frontDeskType
                .GetProperties()
                .FirstOrDefault(pi => pi.PropertyType.IsAssignableFrom(typeof(ReadOnlyObservableCollection<IOrder>)));
            _ordersBackingField = _frontDeskType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(fi => fi.FieldType.IsAssignableFrom(typeof(ObservableCollection<IOrder>)));
            _eventInfo = _frontDeskType.GetEvent("OrderCreated");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _frontDesk = new FrontDesk();
        }

        [MonitoredTest("Order - Should have an OrderCreated event"), Order(1)]
        public void _01_ShouldHaveAnOrderCreatedEvent()
        {
            AssertHasOrderCreatedEvent();
        }

        [MonitoredTest("Order - Should have a property of type ReadOnlyObservableCollection<IOrder> backed by an ObservableCollection field"), Order(2)]
        public void _02_ShouldHaveAPropertyOfTypeReadOnlyObservableCollectionOfIOrderBackedByAnObservableCollectionField()
        {
            AssertHasOrdersPropertyWithBackingField();
        }

        [MonitoredTest("Order - Constructor - Should initialize Orders collection"), Order(3)]
        public void _03_Constructor_ShouldInitializeOrdersCollection()
        {
            AssertHasOrdersPropertyWithBackingField();

            var readOnlyCollection = (ReadOnlyObservableCollection<IOrder>)_ordersProperty.GetValue(_frontDesk);
            var collection = (ObservableCollection<IOrder>)_ordersBackingField.GetValue(_frontDesk);

            Assert.That(readOnlyCollection, Is.Not.Null, "The ReadOnlyObservableCollection should not be null after construction.");
            Assert.That(collection, Is.Not.Null, "The ObservableCollection backing field should not be null after construction.");
            Assert.That(readOnlyCollection.Count, Is.Zero, "The ReadOnlyObservableCollection should be empty after construction.");
            Assert.That(collection.Count, Is.Zero, "The ObservableCollection backing field should be empty after construction.");

            IOrder order = new Order(1) as IOrder;
            Assert.That(order, Is.Not.Null,
                "Cannot create an order. Please make sure that the tests on the 'Order' class are green.");
            collection.Add(order);
            Assert.That(readOnlyCollection, Contains.Item(order), "The ReadOnlyObservableCollection should be a wrapper around the backing field. " +
                                                                  "(When an order is added to the backing field it should also be in the readonly collection.)");
        }

        [MonitoredTest("Order - AddOrder - Should add an order to the orders collection and invoke the 'OrderCreated' event."), Order(4)]
        public void _04_AddOrder_ShouldAddAnOrderToTheOrdersCollectionAndInvokeTheOrderCreatedEvent()
        {
            AssertHasOrdersPropertyWithBackingField();
            AssertHasOrderCreatedEvent();

            //Arrange
            bool eventRaised = false;
            int numberOfBurgers = Random.Next(1, 11);

            _eventInfo.AddEventHandler(_frontDesk, new OrderCreatedHandler((sender, args) =>
            {
                eventRaised = true;
                Assert.That(args.Order.NumberOfBurgers, Is.EqualTo(numberOfBurgers), "The 'NumberOfBurgers' of the added order is not correct.");
            }));

            //Act
            _frontDesk.AddOrder(numberOfBurgers);

            //Assert
            var orders = (ReadOnlyObservableCollection<IOrder>)_ordersProperty.GetValue(_frontDesk);
            Assert.That(orders.Count, Is.EqualTo(1), "The added order cannot be found in the 'Orders' property.");
            Assert.That(orders.First().NumberOfBurgers, Is.EqualTo(numberOfBurgers), "The 'NumberOfBurgers' of the added order is not correct.");
            Assert.That(eventRaised, Is.True, "The 'OrderCreated' event was not invoked.");
        }

        [MonitoredTest("Order - RemoveCompletedOrders - Should remove orders that are completed"), Order(5)]
        public void _05_RemoveCompletedOrders_ShouldRemoveOrdersThatAreCompleted()
        {
            AssertHasOrdersPropertyWithBackingField();

            //Arrange
            var orders = (ObservableCollection<IOrder>)_ordersBackingField.GetValue(_frontDesk);
            int numberOfUnCompletedOrders = Random.Next(2, 11);
            int numberOfCompletedOrders = Random.Next(2, 11);

            for (int i = 0; i < numberOfCompletedOrders; i++)
            {
                IOrder order = new Order(1) as IOrder;
                Assert.That(order, Is.Not.Null, "'Order' does not implement 'IOrder'.");
                order.IsCompleted = true;
                orders.Add(order);
            }

            for (int i = 0; i < numberOfUnCompletedOrders; i++)
            {
                IOrder order = new Order(1) as IOrder;
                Assert.That(order, Is.Not.Null, "'Order' does not implement 'IOrder'.");
                order.IsCompleted = false;
                orders.Add(order);
            }

            //Act
            _frontDesk.RemoveCompletedOrders();

            //Assert
            var readonlyOrders = (ReadOnlyObservableCollection<IOrder>)_ordersProperty.GetValue(_frontDesk);
            Assert.That(readonlyOrders.Count, Is.EqualTo(numberOfUnCompletedOrders));
            Assert.That(readonlyOrders.All(o => o.IsCompleted == false));
        }

        private void AssertHasOrdersPropertyWithBackingField()
        {
            Assert.That(_ordersProperty, Is.Not.Null,
                "No public property of type 'ReadOnlyObservableCollection<IOrder>' is found.");
            Assert.That(_ordersProperty.SetMethod, Is.Null, "The Orders property should not have a setter.");
            Assert.That(_ordersBackingField, Is.Not.Null,
                "No private backing field of type 'ObservableCollection<IOrder>' is found.");
            Assert.That(_ordersBackingField.IsInitOnly, Is.True, "The _orders backing field should be readonly.");
        }

        private void AssertHasOrderCreatedEvent()
        {
            Assert.That(_eventInfo, Is.Not.Null, "No public event with the name 'OrderCreated' is found");
        }
    }
}