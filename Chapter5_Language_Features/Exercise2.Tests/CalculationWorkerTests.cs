using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise2.Tests
{
    [ExerciseTestFixture("dotnet2", "H05", "Exercise02", 
        @"Exercise2\CalculationWorker.cs;
Exercise2\CalculationCompleteHandler.cs;
Exercise2\CalculationEventArgs.cs")]
    public class CalculationWorkerTests
    {
        private CalculationWorker _worker;
        private List<CalculationEventArgs> _receivedEventArgs;
        private Random _random;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _random = new Random();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _worker = new CalculationWorker();
            _receivedEventArgs = new List<CalculationEventArgs>();
        }

        [MonitoredTest("CalculationWorker - Should have a CalculationCompleted event")]
        public void ShouldHaveACalculationCompletedEvent()
        {
            EventHelper.AssertAndRetrieveEventInfo();
        }

        [MonitoredTest("CalculationCompleteHandler - CalculationEventArgs - Should not have been changed")]
        public void CalculationCompleteHandler_CalculationEventArgs_ShouldNotHaveBeenChanged()
        {
            string calculationCompleteHandlerContentHash = Solution.Current.GetFileHash(@"Exercise2\CalculationCompleteHandler.cs");
            Assert.That(calculationCompleteHandlerContentHash, Is.EqualTo("32-69-9A-7B-CD-E5-FF-81-1B-7C-68-97-48-0E-AE-0F"));

            string calculationEventArgsContentHash = Solution.Current.GetFileHash(@"Exercise2\CalculationEventArgs.cs");
            Assert.That(calculationEventArgsContentHash, Is.EqualTo("00-FB-FD-6E-5C-BF-F2-08-D9-7B-02-36-54-C3-9C-C6"));
        }

        [MonitoredTest("CalculationWorker - DoWork - Should invoke the CalculationCompleted event after each calculation")]
        public void DoWork_ShouldInvokeTheCalculationCompletedEventAfterEachCalculation()
        {
            //Arrange
            EventInfo eventInfo = EventHelper.AssertAndRetrieveEventInfo();
            eventInfo.AddEventHandler(_worker, new CalculationCompleteHandler(CalculationCompleted));

            var inputs = new List<int>();
            var expectedOutputs = new List<long>();
            var expectedProgresses = new List<double>();
            int numberOfInputs = _random.Next(2, 11);

            double progressStep = 1.0 / numberOfInputs;
            double progress = 0.0;
            for (int i = 0; i < numberOfInputs; i++)
            {
                int input = _random.Next(2, 1001);
                inputs.Add(input);
                expectedOutputs.Add(input + 1);
                progress += progressStep;
                expectedProgresses.Add(progress);
            }

            //Act
            _worker.DoWork(inputs.ToArray(), n => n + 1);

            Thread.Sleep(150); //wait for the task to complete.

            //Assert
            Assert.That(_receivedEventArgs.Count, Is.EqualTo(inputs.Count), "The event was not triggered as many times as expected.");
            Assert.That(_receivedEventArgs.Select(arg => arg.Result), Is.EquivalentTo(expectedOutputs),
                "The calculation results are unexpected. " +
                "Make sure to invoke the math operation delegate for each input.");

            for (int i = 0; i < _receivedEventArgs.Count; i++)
            {
                double actualProgress = _receivedEventArgs[i].ProgressPercentage;
                double expectedProgress = expectedProgresses[i];
                Assert.That(actualProgress, Is.EqualTo(expectedProgress).Within(0.001), $"Unexpected progress percentage for the {i + 1}th input");
            }
        }

        [MonitoredTest("CalculationWorker - DoWork - Should not invoke the CalculationCompleted event when there are so subscribers")]
        public void DoWork_NoSubscribersOnTheCalculationCompletedEvent_ShouldNotInvokeTheEvent()
        {
            EventHelper.AssertAndRetrieveEventInfo();

            //Act
            _worker.DoWork(new []{1,2,3}, n => n * 2);

            Thread.Sleep(100); //wait for the task to complete.

            //Assert
            Assert.That(_receivedEventArgs.Count, Is.Zero);
        }

        private void CalculationCompleted(object sender, CalculationEventArgs args)
        {
            Assert.That(sender, Is.SameAs(_worker), "The sender should be the instance that invoked the event.");
            _receivedEventArgs.Add(args);
        }
    }
}