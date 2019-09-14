using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Exercise3.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise03", 
        @"Exercise3\MainWindow.xaml;Exercise3\MainWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private RepeatButton _growButton;
        private RepeatButton _shrinkButton;
        private Canvas _canvas;
        private Rectangle _rectangle;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            var allButtons = _window.GetUIElements<RepeatButton>().ToList();
            _growButton = allButtons.FirstOrDefault(button => (button.Content as string)?.ToLower() == "grow");
            _shrinkButton = allButtons.FirstOrDefault(button => (button.Content as string)?.ToLower() == "shrink");

            _canvas = _window.GetUIElements<Canvas>().FirstOrDefault();
            if (_canvas != null)
            {
                _rectangle = _canvas.Children.OfType<Rectangle>().FirstOrDefault();
            }

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should have 2 (repeat)buttons"), Order(1)]
        public void _1_ShouldHaveTwoRepeatButtons()
        {
            AssertHasButtons();
        }

        [MonitoredTest("Should have a canvas that contains a rectangle"), Order(2)]
        public void _2_ShouldHaveACanvasThatContainsARectangle()
        {
            AssertHasCanvasWithRectangle();
        }

        [MonitoredTest("Should increase the width of the rectangle when the grow button is hold down"), Order(3)]
        public void _3_ShouldIncreaseTheRectangleWidthWhenHoldingTheGrowButton()
        {
            AssertHasButtons();
            AssertHasCanvasWithRectangle();

            AssertGrowsAfterClickEvent("The rectangle does not grow.");
            AssertGrowsAfterClickEvent("The rectangle does not grow further when holding the button.");
        }

        [MonitoredTest("Should decrease the width of the rectangle when the shrink button is hold down"), Order(4)]
        public void _4_ShouldDecreaseTheRectangleWidthWhenHoldingTheShrinkButton()
        {
            AssertHasButtons();
            AssertHasCanvasWithRectangle();

            AssertShrinksAfterClickEvent("The rectangle does not shrink.");
            AssertShrinksAfterClickEvent("The rectangle does not shrink further when holding the button.");
        }

        [MonitoredTest("Should not decrease the width of the rectangle under zero"), Order(5)]
        public void _5_ShouldNotDecreaseTheWithOfTheRectangleUnderZero()
        {
            AssertHasButtons();
            AssertHasCanvasWithRectangle();

            _rectangle.Width = 0;
            _shrinkButton.FireClickEvent();
            var newWidth = _rectangle.Width;

            Assert.That(newWidth, Is.Zero,
                () =>
                    "The width of the rectangle may not shrink to a value less than zero.");
        }


        [MonitoredTest("Should not increase the width of the rectangle pass the canvas border"), Order(6)]
        public void _6_ShouldNotIncreaseTheWidhtOfTheRectanglePassTheCanvasBorder()
        {
            AssertHasButtons();
            AssertHasCanvasWithRectangle();

            var maxRectangleWidth = _canvas.Width - _rectangle.Margin.Left;
            _rectangle.Width = maxRectangleWidth;
            _growButton.FireClickEvent();
            var newWidth = _rectangle.Width;

            Assert.That(newWidth, Is.EqualTo(maxRectangleWidth),
                () =>
                    "The rectangle may not grow wider than the width of the canvas minus the x-position of the rectangle");
        }

        private void AssertHasButtons()
        {
            Assert.That(_growButton, Is.Not.Null, () => "A 'RepeatButton' with the text 'Grow' as 'Content' could not be found.");
            Assert.That(_shrinkButton, Is.Not.Null, () => "A 'RepeatButton' with the text 'Shrink' as 'Content' could not be found.");
        }

        private void AssertHasCanvasWithRectangle()
        {
            Assert.That(_canvas, Is.Not.Null, () => "No 'Canvas' layout control could be found.");
            Assert.That(_rectangle, Is.Not.Null, () => "The 'Canvas' does not contain a 'Rectangle'.");
        }

        private void AssertGrowsAfterClickEvent(string failureMessage)
        {
            var originalWidth = _rectangle.Width;
            _growButton.FireClickEvent();
            var newWidth = _rectangle.Width;
            Assert.That(newWidth, Is.GreaterThan(originalWidth), () => failureMessage);
        }

        private void AssertShrinksAfterClickEvent(string failureMessage)
        {
            var originalWidth = _rectangle.Width;
            _shrinkButton.FireClickEvent();
            var newWidth = _rectangle.Width;
            Assert.That(newWidth, Is.LessThan(originalWidth), () => failureMessage);
        }
    }
}
