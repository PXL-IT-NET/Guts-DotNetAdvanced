using System;
using System.Reflection;
using NUnit.Framework;

namespace PlumberApp.Tests
{
    public abstract class TestBase
    {
        protected static Random Random = new Random();

        protected void AssertHasPrivateSetter(Type type, string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName);
            Assert.That(property, Is.Not.Null, $"No property '{propertyName}' was found");
            Assert.That(property.SetMethod, Is.Not.Null, $"No setter found for the '{propertyName}' property");
            Assert.That(property.SetMethod.IsPrivate, Is.True,
                $"'{propertyName}' does not have a private setter. Specify the 'private' keyword right before the 'set' keyword.");
        }
    }
}