using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise2.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise02", @"Exercise2\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Button _normalButton;
        private Button _bigButton;
        private Button _disabledButton;
        private Style _hulkStyle;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            var allButtons = _window.GetUIElements<Button>().ToList();
            if (allButtons.All(button => button.Parent is Grid && button.VerticalAlignment == VerticalAlignment.Top))
            {
                allButtons = allButtons.OrderBy(button => button.Margin.Top).ToList();
            }

            if (allButtons.Count >= 1)
            {
                _normalButton = allButtons.ElementAt(0);
            }
            if (allButtons.Count >= 2)
            {
                _bigButton = allButtons.ElementAt(1);
            }
            if (allButtons.Count >= 3)
            {
                _disabledButton = allButtons.ElementAt(2);
            }

            _hulkStyle = _window.Window.Resources.Values.OfType<Style>().FirstOrDefault();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _1_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise2\MainWindow.xaml.cs";

            var fileContent = Solution.Current.GetFileContent(codeBehindFilePath);
            Assert.That(fileContent.Length, Is.LessThanOrEqualTo(200), () => $"The file '{codeBehindFilePath}' has changed. " +
                                                                             "Undo your changes on the file to make this test pass. " +
                                                                             "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("Should have 3 buttons"), Order(2)]
        public void _2_ShouldHaveThreeButtons()
        {
            AssertHasButtons();
        }

        [MonitoredTest("Should have a style resource that targets buttons"), Order(3)]
        public void _3_ShouldHaveAStyleResourceThatTargetsButtons()
        {
            AssertHasStyle();
        }

        [MonitoredTest("Should have a style resource that sets the background to a linear gradient brush"), Order(4)]
        public void _4_ShouldHaveAStyleResourceThatSetsTheBackgroundToALinearGradientBrush()
        {
            AssertHasStyle();
            GetAndAssertBackgroundSetter();
        }

        [MonitoredTest("Should have the linear gradient brush of the button style correctly defined"), Order(5)]
        public void _5_ShouldHaveTheLinearGradientBrushCorrectlyDefined()
        {
            AssertHasStyle();
            var backgroundSetter = GetAndAssertBackgroundSetter();
            var brush = (LinearGradientBrush)backgroundSetter.Value;

            Assert.That(brush.StartPoint.X, Is.EqualTo(brush.EndPoint.X),
                () =>
                    "Since the gradient should flow vertically the X coordinates of the 'StartPoint' and 'EndPoint' should be the same.");
            Assert.That(brush.StartPoint.Y, Is.EqualTo(0),
                () =>
                    "Since the gradient should flow vertically the Y coordinate of the 'StartPoint' should be 0.");
            Assert.That(brush.EndPoint.Y, Is.EqualTo(1),
                () =>
                    "Since the gradient should flow vertically the Y coordinate of the 'EndPoint' should be 1.");

            Assert.That(brush.GradientStops, Has.Count.EqualTo(3), () => "The gradient brush should have 3 gradient stops");
            var firstStop = brush.GradientStops[0];
            var middleStop = brush.GradientStops[1];
            var lastStop = brush.GradientStops[2];
            Assert.That(firstStop.Color, Is.EqualTo(lastStop.Color),
                () => "The color of the first and last 'GradientStop' should both be the same.");
            Assert.That(firstStop.Color, Is.Not.EqualTo(middleStop.Color),
                () => "The color of the middle 'GradientStop' should be different than the first and last stops.");
            Assert.That(firstStop.Offset, Is.EqualTo(0.0), () => "The first 'GradientStop' should have an 'Offset' of 0.");
            Assert.That(middleStop.Offset, Is.EqualTo(0.5), () => "The middle 'GradientStop' should have an 'Offset' of 0.5.");
            Assert.That(lastStop.Offset, Is.EqualTo(1.0), () => "The last 'GradientStop' should have an 'Offset' of 1.0.");
        }

        [MonitoredTest("Should use the defined style for the 3 buttons"), Order(6)]
        public void _6_ShouldUseTheDefinedStyleForTheButtons()
        {
            AssertHasButtons();
            AssertHasStyle();
            Assert.That(_normalButton.Style, Is.EqualTo(_hulkStyle),
                () => "The 'Style' property of the top button is not set correctly.");
            Assert.That(_bigButton.Style, Is.EqualTo(_hulkStyle),
                () => "The 'Style' property of the middle button is not set correctly.");
            Assert.That(_disabledButton.Style, Is.EqualTo(_hulkStyle),
                () => "The 'Style' property of the bottom button is not set correctly.");
        }

        [MonitoredTest("Should have a button with a bigger fontsize in the middle"), Order(7)]
        public void _7_ShouldHaveAButtonWithABiggerFontsizeInTheMiddle()
        {
            AssertHasButtons();
            Assert.That(_bigButton.FontSize, Is.GreaterThan(12),
                () => "The 'FontSize' of the middle button should be bigger. E.g. 22.");
        }

        [MonitoredTest("Should have a disabled button at the bottom"), Order(8)]
        public void _8_ShouldHaveADisabledButtonAtTheBottom()
        {
            AssertHasButtons();
            Assert.That(_disabledButton.IsEnabled, Is.False);
        }

        private void AssertHasButtons()
        {
            Assert.That(_normalButton, Is.Not.Null, () => "The button on the top could not be found.");
            Assert.That(_bigButton, Is.Not.Null, () => "The button in the middle could not be found.");
            Assert.That(_disabledButton, Is.Not.Null, () => "The button at the bottom could not be found.");
        }

        private void AssertHasStyle()
        {
            Assert.That(_hulkStyle, Is.Not.Null,
                () => "The 'Resources' collection of the window should contain an instance of 'Style'.");
            Assert.That(_hulkStyle.TargetType.Name, Is.EqualTo("Button"),
                () => "A 'Style' instance was found but it does not target buttons ('TargetType').");
        }

        private Setter GetAndAssertBackgroundSetter()
        {
            var backgroundSetter = _hulkStyle.Setters.OfType<Setter>()
                .FirstOrDefault(s => s.Property.Name.ToLower() == "background");
            Assert.That(backgroundSetter, Is.Not.Null,
                () => "No 'Setter' that targets the 'Background' property could be found.");
            Assert.That(backgroundSetter.Value, Is.TypeOf<LinearGradientBrush>(),
                () => "The 'Value' of the 'Setter' should be an instance of 'LinearGradientBrush'.");
            return backgroundSetter;
        }
    }
}
