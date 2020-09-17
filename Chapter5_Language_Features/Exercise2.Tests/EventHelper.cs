using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Exercise2.Tests
{
    public static class EventHelper
    {
        public static EventInfo AssertAndRetrieveEventInfo()
        {
            var assembly = Assembly.GetAssembly(typeof(CalculationWorker));
            var calculationWorkerTypeInfo = assembly.DefinedTypes.First(t => t.AsType() == typeof(CalculationWorker));

            EventInfo eventInfo = calculationWorkerTypeInfo.DeclaredEvents.FirstOrDefault();
            Assert.That(eventInfo, Is.Not.Null, "No event found in the 'CalculationWorker' class.");

            Assert.That(eventInfo.EventHandlerType, Is.EqualTo(typeof(CalculationCompleteHandler)),
                "The event in the 'CalculationWorker' class should be of type 'CalculationCompleteHandler'.");

            return eventInfo;
        }
    }
}