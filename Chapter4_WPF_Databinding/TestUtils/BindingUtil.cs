using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using NUnit.Framework;

namespace TestUtils
{
    public static class BindingUtil
    {
        public static void AssertBinding(FrameworkElement targetElement, DependencyProperty targetProperty,
            string expectedBindingPath, BindingMode allowedBindingMode)
        {
            BindingExpression binding = targetElement.GetBindingExpression(targetProperty);
            var errorMessage =
                $"Invalid 'Binding' for the '{targetProperty.Name}' property of {targetElement.Name}.";
            Assert.That(binding, Is.Not.Null, errorMessage);
            Assert.That(binding.ParentBinding.Path.Path, Is.EqualTo(expectedBindingPath), errorMessage);

            var allowedBindingModes = new List<BindingMode> { allowedBindingMode };
            var metaData = (FrameworkPropertyMetadata)targetProperty.GetMetadata(targetElement);
            if (allowedBindingMode == BindingMode.TwoWay && metaData.BindsTwoWayByDefault)
            {
                allowedBindingModes.Add(BindingMode.Default);
            }
            else if (allowedBindingMode == BindingMode.OneWay)
            {
                allowedBindingModes.Add(BindingMode.Default);
            }
            Assert.That(allowedBindingModes, Has.One.EqualTo(binding.ParentBinding.Mode), errorMessage);
        }

    }
}