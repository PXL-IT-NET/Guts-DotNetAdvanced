using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise9.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise09", @"Exercise9\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private CheckBox _simpleCheckBox;
        private CheckBox _imageCheckBox;
        private CheckBox _gradientCheckBox;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            var allCheckBoxes = _window.GetUIElements<CheckBox>().ToList();
            if (allCheckBoxes.All(checkBox => checkBox.Parent is Grid && checkBox.VerticalAlignment == VerticalAlignment.Top))
            {
                allCheckBoxes = allCheckBoxes.OrderBy(checkBox => checkBox.Margin.Top).ToList();
            }

            if (allCheckBoxes.Count >= 1)
            {
                _simpleCheckBox = allCheckBoxes.ElementAt(0);
            }
            if (allCheckBoxes.Count >= 2)
            {
                _imageCheckBox = allCheckBoxes.ElementAt(1);
            }
            if (allCheckBoxes.Count >= 3)
            {
                _gradientCheckBox = allCheckBoxes.ElementAt(2);
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
            var codeBehindFilePath = @"Exercise9\MainWindow.xaml.cs";

            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("F8-F8-02-42-9B-C6-49-C4-62-59-CB-6C-2E-12-04-4C"), () =>
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Should have a simple checkbox on the top"), Order(2)]
        public void _2_ShouldHaveASimpleCheckBoxOnTheTop()
        {
            Assert.That(_simpleCheckBox, Is.Not.Null, () => "The first checkbox (on the top) could not be found.");
            Assert.That(_simpleCheckBox.Content, Is.TypeOf<string>(), () => "The content of first checkbox (on the top) should be a string.");
            Assert.That(_simpleCheckBox.Content, Is.Not.Empty, () => "The content of first checkbox (on the top) should not be empty.");
        }

        [MonitoredTest("Should have a checkbox with an image and text in the middle"), Order(3)]
        public void _3_ShouldHaveAnImageCheckBoxInTheMiddle()
        {
            Assert.That(_imageCheckBox, Is.Not.Null, () => "The second (middle) checkbox could not be found.");
            Assert.That(_imageCheckBox.Content, Is.TypeOf<StackPanel>(),
                () =>
                    "The content of the middle checkbox should be a 'StackPanel' so that you can position the image on the left of the text.");
            var contentStackPanel = (StackPanel)_imageCheckBox.Content;
            Assert.That(contentStackPanel.Orientation, Is.EqualTo(Orientation.Horizontal),
                () => "The 'StackPanel' of the middle checkbox should have a horizontal 'Orientation'.");
            Assert.That(contentStackPanel.Children.Count, Is.EqualTo(2),
                () => "The 'StackPanel' of the middle checkbox should have 2 child controls.");
            var imageControl = contentStackPanel.Children[0] as Image;
            Assert.That(imageControl, Is.Not.Null,
                () => "The first child control of the 'StackPanel' of the middle checkbox should be an 'Image' control.");
            Assert.That(imageControl.Source, Is.Not.Null,
                () => "The 'Image' must have a (valid) 'Source'.");

            var bitmapSource = imageControl.Source as BitmapFrame;
            Assert.That(bitmapSource, Is.Not.Null,
                () => "The 'Source' of 'Image' must be a valid (bitmap) image.");

            Assert.That(bitmapSource.BaseUri, Is.Not.Null,
                () => "The 'Source' of the 'Image' in the middle checkbox should be a relative path.");

            Assert.That(bitmapSource.ToString(), Contains.Substring("kameleon.jpg").IgnoreCase,
                () => "The 'Image' in the middle checkbox should have its 'Source' set to 'images\\kameleon.jpg'.");

            var textBlockControl = contentStackPanel.Children[1] as TextBlock;
            Assert.That(textBlockControl, Is.Not.Null,
                () =>
                    "The second child control of the 'StackPanel' of the middle checkbox should be a 'TextBlock' control.");
            Assert.That(textBlockControl.FontSize, Is.GreaterThanOrEqualTo(14),
                () => "The 'FontSize' in the 'TextBlock' in the middle checkbox should be bigger (e.g. 16).");
            Assert.That(textBlockControl.FontWeight.ToString(), Is.EqualTo("Bold").IgnoreCase,
                () => "The 'FontWeight' in the 'TextBlock' in the middle checkbox should be bold.");
        }

        [MonitoredTest("Should have a checkbox on the bottom with backgrounds"), Order(4)]
        public void _4_ShouldHaveACheckBoxWithBackgroundsAtTheBottom()
        {
            Assert.That(_gradientCheckBox, Is.Not.Null, () => "The third (bottom) checkbox could not be found.");
           
            Assert.That(_gradientCheckBox.Background.ToString(), Contains.Substring("FFFF00").IgnoreCase, () => "The color of the background of the bottom checkbox should be 'Yellow'.");

            var textBlockControl = _gradientCheckBox.Content as TextBlock;
            Assert.That(textBlockControl, Is.Not.Null, () => "The content of the bottom checkbox should be a TextBlock (So that a gradient background can be set).");

            Assert.That(textBlockControl.Foreground.ToString(), Contains.Substring("FFFFFF").IgnoreCase, () => "The color of the text ('ForeColor') of the bottom checkbox should be 'White'.");
            Assert.That(textBlockControl.FontWeight.ToString(), Is.EqualTo("Bold").IgnoreCase, () => "The 'FontWeight' of the text ('ForeColor') of the bottom checkbox should be bold.");

            var backgroundBrush = textBlockControl.Background as LinearGradientBrush;
            Assert.That(backgroundBrush, Is.Not.Null, () => "The 'Background' property of the TextBlock should be an instance of a 'LinearGradientBrush'.");
            Assert.That(backgroundBrush.GradientStops.Count, Is.EqualTo(3), () => "The background brush of the TextBlock should have 3 instances of 'GradientStop'. " +
                                                                                  "There should be a 'GradientStop' for each of the following colors: 'Red', 'Blue', 'Green'.");
            double expectedOffset = 0.0;
            for (var index = 0; index < backgroundBrush.GradientStops.Count; index++)
            {
                var gradientStop = backgroundBrush.GradientStops[index];
                var gradientStopPosition = index + 1;
                var offset = expectedOffset;
                Assert.That(gradientStop.Offset, Is.EqualTo(expectedOffset),
                    () => $"The 'GradientStop' at position {gradientStopPosition} should have an 'Offset' of {offset}");
                expectedOffset += 0.5;
            }
        }
    }
}
