using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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
        private CheckBox _kameleonCheckBox;
        private CheckBox _gradientCheckBox;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            var allCheckBoxes = _window.GetUIElements<CheckBox>().ToList();
            if (allCheckBoxes.All(checkBox =>
                checkBox.Parent is Grid && checkBox.VerticalAlignment == VerticalAlignment.Top))
            {
                allCheckBoxes = allCheckBoxes.OrderBy(checkBox => checkBox.Margin.Top).ToList();
            }

            if (allCheckBoxes.Count >= 1)
            {
                _simpleCheckBox = allCheckBoxes.ElementAt(0);
            }

            if (allCheckBoxes.Count >= 2)
            {
                _kameleonCheckBox = allCheckBoxes.ElementAt(1);
            }

            if (allCheckBoxes.Count >= 3)
            {
                _gradientCheckBox = allCheckBoxes.ElementAt(2);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
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
            Assert.That(_simpleCheckBox.Content, Is.TypeOf<string>(),
                () => "The content of first checkbox (on the top) should be a string.");
            Assert.That(_simpleCheckBox.Content, Is.Not.Empty,
                () => "The content of first checkbox (on the top) should not be empty.");
        }

        [MonitoredTest("Should have a kameleon checkbox in the middle"), Order(3)]
        public void _3_ShouldHaveAKameleonCheckBoxInTheMiddle()
        {
            Assert.That(_kameleonCheckBox, Is.Not.Null, () => "The second (middle) checkbox could not be found.");

            Assert.That(_kameleonCheckBox.Background, Is.Not.Null, "The checkbox must have a background set.");
            Assert.That(_kameleonCheckBox.Background, Is.TypeOf<SolidColorBrush>(),
                "The background of the checkbox must be a 'SolidColorBrush'.");

            Assert.That(_kameleonCheckBox.Content, Is.TypeOf<Border>(),
                "The content of the middle checkbox should be a 'Border' so that a rounded border can be displayed around the inner controls.");

            var border = (Border)_kameleonCheckBox.Content;
            bool borderThicknessIsEqualOnAllSides =
                IsEqualThicknessOnAllSides(border.BorderThickness, out double borderSize);
            Assert.That(borderSize, Is.GreaterThan(0.0), "The border must have a thickness.");
            Assert.That(borderThicknessIsEqualOnAllSides, Is.True,
                "The border thickness should be the same on all sides.");

            Assert.That(border.BorderBrush, Is.Not.Null, "The border must have a brush set.");
            Assert.That(border.BorderBrush, Is.TypeOf<SolidColorBrush>(),
                "The border brush must be a 'SolidColorBrush'.");

            bool borderPaddingIsEqualOnAllSides = IsEqualThicknessOnAllSides(border.Padding, out double paddingSize);
            Assert.That(paddingSize, Is.GreaterThan(0.0), "The border must have some padding.");
            Assert.That(borderPaddingIsEqualOnAllSides, Is.True, "The border must have the same padding on each side.");

            Assert.That(border.CornerRadius.TopLeft, Is.GreaterThan(0.0), "The border must have a corner radius.");
            Assert.That(border.CornerRadius.TopRight, Is.GreaterThan(0.0), "The border must have a corner radius.");
            Assert.That(border.CornerRadius.BottomLeft, Is.GreaterThan(0.0), "The border must have a corner radius.");
            Assert.That(border.CornerRadius.BottomRight, Is.GreaterThan(0.0), "The border must have a corner radius.");

            Assert.That(border.Child, Is.TypeOf<StackPanel>(),
                () =>
                    "The content (child) of the border should be a 'StackPanel' so that you can position the image on the left of the text.");
            var contentStackPanel = (StackPanel)border.Child;
            Assert.That(contentStackPanel.Orientation, Is.EqualTo(Orientation.Vertical),
                () => "The 'StackPanel' of the middle checkbox should have a vertical 'Orientation'.");
            Assert.That(contentStackPanel.Children.Count, Is.EqualTo(2),
                () => "The 'StackPanel' of the middle checkbox should have 2 child controls.");
            var imageControl = contentStackPanel.Children[0] as Image;
            Assert.That(imageControl, Is.Not.Null,
                () =>
                    "The first child control of the 'StackPanel' of the middle checkbox should be an 'Image' control.");
            Assert.That(imageControl.Source, Is.Not.Null,
                () => "The 'Image' must have a (valid) 'Source'.");

            var bitmapSource = imageControl.Source as BitmapFrame;
            Assert.That(bitmapSource, Is.Not.Null,
                () => "The 'Source' of 'Image' must be a valid (bitmap) image.");

            Assert.That(bitmapSource.BaseUri, Is.Not.Null,
                () => "The 'Source' of the 'Image' in the middle checkbox should be a relative path.");

            Assert.That(bitmapSource.ToString(), Contains.Substring("kameleon.jpg").IgnoreCase,
                () => "The 'Image' in the middle checkbox should have its 'Source' set to 'images\\kameleon.jpg'.");

            var innerStackPanel = contentStackPanel.Children[1] as StackPanel;
            Assert.That(innerStackPanel, Is.Not.Null,
                "The second child control of the 'StackPanel' of the middle checkbox should be another 'StackPanel' control.");
            Assert.That(innerStackPanel.Orientation, Is.EqualTo(Orientation.Horizontal),
                "The inner 'StackPanel' that contains the text, should have a horizontal 'Orientation'.");

            Assert.That(innerStackPanel.Children.OfType<TextBlock>().Count(), Is.EqualTo(2),
                "The inner 'StackPanel' that contains the text, should have exactly 2 'TextBlock' elements.");

            var leftTextBlock = (TextBlock)innerStackPanel.Children[0];
            Assert.That(leftTextBlock.Foreground, Is.Not.Null, "The left 'TextBlock' should have colored text.");
            Assert.That(leftTextBlock.Text, Is.Not.Null.And.Not.Empty, "The left 'TextBlock' should not be empty.");

            var rightTextBlock = (TextBlock)innerStackPanel.Children[1];
            Assert.That(rightTextBlock.Foreground, Is.Not.Null, "The right 'TextBlock' should have colored text.");
            Assert.That(rightTextBlock.Text, Is.Not.Null.And.Not.Empty, "The right 'TextBlock' should not be empty.");

            double padRight = Math.Max(leftTextBlock.Padding.Right, leftTextBlock.Margin.Right);
            double padLeft = Math.Max(rightTextBlock.Padding.Left, rightTextBlock.Margin.Left);
            Assert.That(padLeft > 0 || padRight > 0, Is.True,
                "There must be some space between the 2 'TextBlock' elements.");
        }

        [MonitoredTest("Should have a checkbox on the bottom with backgrounds"), Order(4)]
        public void _4_ShouldHaveACheckBoxWithBackgroundsAtTheBottom()
        {
            Assert.That(_gradientCheckBox, Is.Not.Null, "The third (bottom) checkbox could not be found.");

            var textBlockControl = _gradientCheckBox.Content as TextBlock;
            Assert.That(textBlockControl, Is.Not.Null,
                "The content of the bottom checkbox should be a TextBlock (So that a gradient background can be set).");

            Assert.That(textBlockControl.Foreground.ToString(), Contains.Substring("FFFFFF").IgnoreCase,
                "The color of the text ('ForeColor') of the bottom checkbox should be 'White'.");
            Assert.That(textBlockControl.FontWeight.ToString(), Is.EqualTo("Bold").IgnoreCase,
                "The 'FontWeight' of the text ('ForeColor') of the bottom checkbox should be bold.");

            var backgroundBrush = textBlockControl.Background as LinearGradientBrush;
            Assert.That(backgroundBrush, Is.Not.Null,
                () => "The 'Background' property of the TextBlock should be an instance of a 'LinearGradientBrush'.");
            Assert.That(backgroundBrush.GradientStops.Count, Is.EqualTo(2), () =>
                "The background brush of the TextBlock should have 2 instances of 'GradientStop'. " +
                "There should be a 'GradientStop' for each of the following colors: 'Red', 'Blue'.");
            double expectedOffset = 0.0;
            for (var index = 0; index < backgroundBrush.GradientStops.Count; index++)
            {
                var gradientStop = backgroundBrush.GradientStops[index];
                var gradientStopPosition = index + 1;
                var offset = expectedOffset;
                Assert.That(gradientStop.Offset, Is.EqualTo(expectedOffset),
                    () => $"The 'GradientStop' at position {gradientStopPosition} should have an 'Offset' of {offset}");
                expectedOffset += 0.9;
            }
        }

        private bool IsEqualThicknessOnAllSides(Thickness thickness, out double valueOnEachSide)
        {
            valueOnEachSide = thickness.Left;
            if (thickness.Top != valueOnEachSide) return false;
            if (thickness.Right != valueOnEachSide) return false;
            if (thickness.Bottom != valueOnEachSide) return false;
            return true;
        }
    }
}
