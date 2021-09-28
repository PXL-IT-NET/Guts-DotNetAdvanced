using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Exercise3.ChefAggregate;
using Exercise3.FrontDeskAggregate;
using Exercise3.OrderAggregate;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;

namespace Exercise3.Tests
{
    [ExerciseTestFixture("dotnet2", "H05", "Exercise03", @"Exercise3\ChefAggregate\Chef.cs")]
    public class ChefTests
    {
        private static readonly Random Random = new Random();
        private FrontDesk _frontDesk;
        private Mock<IChefActions> _chefActionsMock;
        private Chef _chef;
        private FieldInfo _queueField;

        [SetUp]
        public void BeforeEachTest()
        {
            _queueField = typeof(Chef).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(f => f.FieldType.IsAssignableFrom(typeof(Queue<IOrder>)));

            _frontDesk = new FrontDesk();
            _chefActionsMock = new Mock<IChefActions>();
            _chef = new Chef(_frontDesk, _chefActionsMock.Object);
        }

        [MonitoredTest("Chef - Should listen for created orders and add them in a queue")]
        public void ShouldListenForCreatedOrdersAndAddThemInAQueue()
        {
            Queue<IOrder> queue = GetAndAssertQueueFieldValue();
            
            _frontDesk.AddOrder(1);

            Assert.That(queue.Count, Is.EqualTo(1),
                "When the 'AddOrder' method of the 'Frontdesk' is invoked, the queue should contain exactly one order.");
        }

        [MonitoredTest("Chef - StartProcessingOrders - Should process orders in the queue")]
        public void StartProcessingOrders_ShouldProcessOrdersInTheQueue()
        {
            Queue<IOrder> queue = GetAndAssertQueueFieldValue();

            int numberOfOrders = Random.Next(5, 11);
            int burgerTotal = 0;
            var orders = new List<IOrder>();
            for (int i = 0; i < numberOfOrders; i++)
            {
                int numberOfBurgers = Random.Next(1, 5);
                IOrder order = new Order(numberOfBurgers) as IOrder;
                if (order != null)
                {
                    burgerTotal += numberOfBurgers;
                    orders.Add(order);
                    queue.Enqueue(order);
                }
            }

            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            _chef.StartProcessingOrders(cancellationToken);
            Thread.Sleep(150); //wait a little bit so that the queue is completely processed
            cancellationTokenSource.Cancel();

            Assert.That(queue.Count, Is.EqualTo(0),
                $"The queue (with {numberOfOrders} orders) should be empty after a while.");

            Assert.That(orders, Has.All.Matches((IOrder order) => order.IsStarted),
                "All orders should have 'IsStarted' true after processing the queue.");
            Assert.That(orders, Has.All.Matches((IOrder order) => order.IsCompleted),
                "All orders should have 'IsCompleted' true after processing the queue.");

            _chefActionsMock.Verify(a => a.CookBurger(), Times.Exactly(burgerTotal),
                "The 'CookBurger' action should have been called for each burger in each order.");

            _chefActionsMock.Verify(a => a.TakeABreather(), Times.AtLeast(numberOfOrders),
                "The 'TakeABreather' action should have been called for each order (at least).");
        }


        private Queue<IOrder> GetAndAssertQueueFieldValue()
        {
            Assert.That(_queueField, Is.Not.Null, "The class should have a private field of type 'Queue<IOrder>'.");
            var queue = (Queue<IOrder>)_queueField.GetValue(_chef);
            Assert.That(queue, Is.Not.Null, "The private queue is not instantiated (by the constructor).");
            return queue;
        }
    }
}