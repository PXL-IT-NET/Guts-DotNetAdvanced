using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Guts.Client.Classic;

namespace Exercise5.Tests
{
    [ExerciseTestFixture("dotnet2", "H04", "Exercise05", @"Exercise5\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Slider _slider;
        private TextBox _textBox;
        private TextBlock _textBlock;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            _slider = _window.GetUIElements<Slider>().FirstOrDefault();
            _textBox = _window.GetUIElements<TextBox>().FirstOrDefault();
            _textBlock = _window.GetUIElements<TextBlock>().FirstOrDefault(t => (int)t.GetValue(Grid.RowProperty)==3);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
            _window.Dispose();
        }

        [MonitoredTest("The slider should have the correct databinding statement"), Order(1)]
        public void _1_SliderShouldHaveTheCorrectBindingExpression()
        {
            Assert.That(_slider.Minimum, Is.EqualTo(5), "The slider must hava a minimum value of 5");
            Assert.That(_slider.Maximum, Is.EqualTo(100), "The slider must hava a maximum value of 100");
            BindingExpression bindingExpr = BindingOperations.GetBindingExpression(_slider, Slider.ValueProperty);

            Assert.That(bindingExpr, Is.Not.Null, "The Slider Value has to be a databinding statement");
            Assert.That(bindingExpr.ParentBinding.ElementName, Is.Not.Null, "The slider has to use element databinding");
            Assert.That(bindingExpr.ParentBinding.ElementName, Is.EqualTo(_textBox.Name), "The slider has to use element dataBinding to get the value from the Size TextBox");
            Assert.That(bindingExpr.ParentBinding.Path.Path, Is.EqualTo("Text"), "The databinding statement doesn't have the correct Path property");
        }

        [MonitoredTest("The TextBox should have the correct databinding statement"), Order(2)]
        public void _2_TextBoxShouldHaveTheCorrectBindingExpression()
        {
            BindingExpression bindingExpr = BindingOperations.GetBindingExpression(_textBox, TextBox.TextProperty);

            Assert.That(bindingExpr, Is.Not.Null, "The TextBox Text value has to be a databinding statement");
            Assert.That(bindingExpr.ParentBinding.ElementName, Is.Not.Null, "The textbox has to use element databinding");
            Assert.That(bindingExpr.ParentBinding.ElementName, Is.EqualTo(_slider.Name), "The textbox has to use element dataBinding to get the value from the slider");
            Assert.That(bindingExpr.ParentBinding.Path.Path, Is.EqualTo("Value"), "The databinding statement doesn't have the correct Path property");
        }


        [MonitoredTest("The TextBlock should have the correct databinding statement"), Order(3)]
        public void _3_TextBlockShouldHaveTheCorrectBindingExpression()
        {
            BindingExpression bindingExpr = BindingOperations.GetBindingExpression(_textBlock, TextBox.FontSizeProperty);

            Assert.That(bindingExpr, Is.Not.Null, "The FontSize value has to be a databinding statement");
            Assert.That(bindingExpr.ParentBinding.ElementName, Is.Not.Null, "The TextBlock has to use element databinding");
            Assert.That(bindingExpr.ParentBinding.ElementName, Is.EqualTo(_textBox.Name), "The textBlock has to use element dataBinding to get the value from the textBox");
            Assert.That(bindingExpr.ParentBinding.Path.Path, Is.EqualTo("Text"), "The databinding statement doesn't have the correct Path property");
        }
    }
}
