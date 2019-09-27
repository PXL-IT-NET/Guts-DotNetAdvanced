using Guts.Client.Classic.TestTools.WPF;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using Guts.Client.Classic;
using Guts.Client.Shared;
using TestUtils;

namespace Exercise1.Tests
{
    [ExerciseTestFixture("dotnet2", "H04", "Exercise01", 
        @"Exercise1\MainWindow.xaml;Exercise1\MainWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
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

            BindingUtil.AssertBinding(_gameIdTextBox, TextBox.TextProperty, "GameId", BindingMode.OneWay);
            BindingUtil.AssertBinding(_nameTextBox, TextBox.TextProperty, "Name", BindingMode.OneWay);
            BindingUtil.AssertBinding(_typeTextBox, TextBox.TextProperty, "Type", BindingMode.OneWay);
            BindingUtil.AssertBinding(_releaseDateTextBox, TextBox.TextProperty, "ReleaseDate", BindingMode.OneWay);
            BindingUtil.AssertBinding(_descriptionTextBox, TextBox.TextProperty, "Description", BindingMode.OneWay);
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
