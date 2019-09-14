using System.Linq;
using System.Threading;
using System.Windows;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using NUnit.Framework;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;

namespace Exercise1.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise01", @"Exercise1\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Button _simpleButton;
        private Button _imageButton;
        private Button _gradientButton;

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
                _simpleButton = allButtons.ElementAt(0);
            }
            if (allButtons.Count >= 2)
            {
                _imageButton = allButtons.ElementAt(1);
            }
            if (allButtons.Count >= 3)
            {
                _gradientButton = allButtons.ElementAt(2);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _1_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise1\MainWindow.xaml.cs";

            var fileContent = Solution.Current.GetFileContent(codeBehindFilePath);
            Assert.That(fileContent.Length, Is.LessThanOrEqualTo(200), () => $"The file '{codeBehindFilePath}' has changed. " +
                                                                             "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Should have a simple button on the top"), Order(2)]
        public void _2_ShouldHaveASimpleButtonOnTheTop()
        {
            Assert.That(_simpleButton, Is.Not.Null, () => "The first button (on the top) could not be found.");
            Assert.That(_simpleButton.Content, Is.TypeOf<string>(), () => "The content of first button (on the top) should be a string.");
            Assert.That(_simpleButton.Content, Is.Not.Empty, () => "The content of first button (on the top) should not be empty.");
        }

        [MonitoredTest("Should have a button with an image and text in the middle"), Order(3)]
        public void _3_ShouldHaveAnImageButtonInTheMiddle()
        {
            Assert.That(_imageButton, Is.Not.Null, () => "The second (middle) button could not be found.");
            Assert.That(_imageButton.Content, Is.TypeOf<StackPanel>(),
                () =>
                    "The content of the middle button should be a 'StackPanel' so that you can position the image above the text.");
            var contentStackPanel = (StackPanel)_imageButton.Content;
            Assert.That(contentStackPanel.Orientation, Is.EqualTo(Orientation.Vertical),
                () => "The 'StackPanel' of the middle button should have a vertical 'Orientation'.");
            Assert.That(contentStackPanel.Children.Count, Is.EqualTo(2),
                () => "The 'StackPanel' of the middle button should have 2 child controls.");
            var imageControl = contentStackPanel.Children[0] as Image;
            Assert.That(imageControl, Is.Not.Null,
                () => "The first child control of the 'StackPanel' of the middle button should be an 'Image' control.");
            Assert.That(imageControl.Source, Is.Not.Null,
                () => "The 'Image' must have a (valid) 'Source'.");

            var bitmapSource = imageControl.Source as BitmapFrame;
            Assert.That(bitmapSource, Is.Not.Null,
                () => "The 'Source' of 'Image' must be a valid (bitmap) image.");

            Assert.That(bitmapSource.BaseUri, Is.Not.Null,
                () => "The 'Source' of the 'Image' in the middle should be a relative path.");

            Assert.That(bitmapSource.ToString(), Contains.Substring("banner.png").IgnoreCase,
                () => "The 'Image' in the middle button should have its 'Source' set to 'images\\banner.png'.");

            var textBlockControl = contentStackPanel.Children[1] as TextBlock;
            Assert.That(textBlockControl, Is.Not.Null,
                () =>
                    "The second child control of the 'StackPanel' of the middle button should be an 'TextBlock' control.");
            Assert.That(textBlockControl.FontSize, Is.GreaterThanOrEqualTo(14),
                () => "The 'FontSize' in the 'TextBlock' in the middle button should be bigger (e.g. 16).");
            Assert.That(textBlockControl.FontWeight.ToString(), Is.EqualTo("Bold").IgnoreCase,
                () => "The 'FontWeight' in the 'TextBlock' in the middle button should be bold.");
        }

        [MonitoredTest("Should have a button on the bottom with a gradient brush as background"), Order(4)]
        public void _4_ShouldHaveAGradientButtonAtTheBottom()
        {
            Assert.That(_gradientButton, Is.Not.Null, () => "The third (bottom) button could not be found.");
            Assert.That(_gradientButton.Content, Is.TypeOf<string>(), () => "The content of the bottom button should be a string.");
            Assert.That(_gradientButton.Foreground.ToString(), Contains.Substring("FFFFFF").IgnoreCase, () => "The color of the text ('ForeColor') of the bottom button should be 'White'.");
            Assert.That(_gradientButton.FontWeight.ToString(), Is.EqualTo("Bold").IgnoreCase, () => "The 'FontWeight' of the text ('ForeColor') of the bottom button should be bold.");
            var backgroundBrush = _gradientButton.Background as LinearGradientBrush;
            Assert.That(backgroundBrush, Is.Not.Null, () => "The 'Background' property of the bottom button should be an instance of a 'LinearGradientBrush'.");
            Assert.That(backgroundBrush.GradientStops.Count, Is.EqualTo(4), () => "The background brush of the bottom button should have 4 instances of 'GradientStop'. " +
                                                                                  "There should be a 'GradientStop' for each of the following colors: 'Yellow', 'Red', 'Blue', 'Green'.");
            double expectedOffset = 0.0;
            for (var index = 0; index < backgroundBrush.GradientStops.Count; index++)
            {
                var gradientStop = backgroundBrush.GradientStops[index];
                var gradientStopPosition = index + 1;
                var offset = expectedOffset;
                Assert.That(gradientStop.Offset, Is.EqualTo(expectedOffset).Within(10.0),
                    () => $"The 'GradientStop' at position {gradientStopPosition} should have an 'Offset' of {offset}");
                expectedOffset += 0.33;
            }
        }
    }
}
