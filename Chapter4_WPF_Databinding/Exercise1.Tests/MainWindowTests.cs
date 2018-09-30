using Guts.Client.Classic.TestTools.WPF;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Guts.Client.Classic;
using Guts.Client.Shared;

namespace Exercise1.Tests
{
    [ExerciseTestFixture("dotnet2", 4, "1", @"Exercise1\MainWindow.xaml;Exercise1\MainWindow.xaml.cs"), 
     Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private TextBox _gameIdTextBox;
        private TextBox _nameTextBox;
        private TextBox _typeTextBox;
        private TextBox _releaseDateTextBox;
        private TextBox _descriptionTextBox;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            var allTextBoxes = _window.GetUIElements<TextBox>().OrderBy(textbox => textbox.Margin.Top).ToList();
            if (allTextBoxes.Count >= 1)
            {
                _gameIdTextBox = allTextBoxes.ElementAt(0);
            }
            if (allTextBoxes.Count >= 2)
            {
                _nameTextBox = allTextBoxes.ElementAt(1);
            }
            if (allTextBoxes.Count >= 3)
            {
                _typeTextBox = allTextBoxes.ElementAt(2);
            }
            if (allTextBoxes.Count >= 4)
            {
                _releaseDateTextBox = allTextBoxes.ElementAt(3);
            }
            if (allTextBoxes.Count >= 5)
            {
                _descriptionTextBox = allTextBoxes.ElementAt(4);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should have five TextBoxes"), Order(1)]
        public void _1_ShouldHaveFiveTextBoxes()
        {
            AssertHasFiveTextBoxes();
        }

        [MonitoredTest("Should have a DataContext set to game"), Order(2)]
        public void _2_ShouldHaveDataContextSetToGame()
        {
            Assert.That(_window.Window.DataContext, Is.TypeOf<Game>(), () => "The Datacontext of the window should be a Game object");
        }

        [MonitoredTest("Should have correct bindings for the TextBoxes"), Order(3)]
        public void _3_ShouldHaveCorrectBindingsForTheTextBoxes()
        {
            AssertHasFiveTextBoxes();

            AssertOnWayBinding(_gameIdTextBox, TextBox.TextProperty, "GameId");
            AssertOnWayBinding(_nameTextBox, TextBox.TextProperty, "Name");
            AssertOnWayBinding(_typeTextBox, TextBox.TextProperty, "Type");
            AssertOnWayBinding(_releaseDateTextBox, TextBox.TextProperty, "ReleaseDate");
            AssertOnWayBinding(_descriptionTextBox, TextBox.TextProperty, "Description");
        }

        private void AssertOnWayBinding(Control targetControl, DependencyProperty targetProperty, string expectedBindingPath)
        {
            BindingExpression binding = targetControl.GetBindingExpression(targetProperty);
            Assert.That(binding, Is.Not.Null, () => $"Could not find a 'Binding' for the '{targetProperty.Name}' property of {targetControl.Name}.");
            Assert.That(binding.ParentBinding.Path.Path, Is.EqualTo(expectedBindingPath),
                () =>
                    $"The source property of the databinding statement of {targetControl.Name} should be the {expectedBindingPath} property of the source object");
            Assert.That(binding.ParentBinding.Mode, Is.AnyOf(BindingMode.Default, BindingMode.OneWay));
        }

        private void AssertHasFiveTextBoxes()
        {
            Assert.That(_gameIdTextBox, Is.Not.Null, () => "The textbox for the game Id could not be found.");
            Assert.That(_nameTextBox, Is.Not.Null, () => "The textbox for the name could not be found.");
            Assert.That(_typeTextBox, Is.Not.Null, () => "The textbox for the type could not be found.");
            Assert.That(_releaseDateTextBox, Is.Not.Null, () => "The textbox for the release date could not be found.");
            Assert.That(_descriptionTextBox, Is.Not.Null, () => "The textbox for the description could not be found.");
        }
    }
}
