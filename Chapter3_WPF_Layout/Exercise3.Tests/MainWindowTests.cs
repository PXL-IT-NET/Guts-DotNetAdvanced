using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise3.Tests
{
    [ExerciseTestFixture("dotnet2", "H03", "Exercise03", @"Exercise3\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Canvas _canvas;
        private IList<Ellipse> _ellipses;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            _canvas = _window.GetUIElements<Canvas>().FirstOrDefault();
            _ellipses = _canvas.FindVisualChildren<Ellipse>().ToList();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _01_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise3\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("6E-B4-E0-0D-58-AB-72-FB-4E-AA-DB-C1-7B-19-AA-F0"),
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass. " +
                "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("Should have 5 ellipses in a Canvas"), Order(2)]
        public void _02_ShouldHaveFiveEllipsesInACanvas()
        {
            Assert.That(_canvas, Is.Not.Null, "Cannot find a 'Canvas' in the window.");
            Assert.That(_canvas.Parent, Is.SameAs(_window.Window), "The 'Canvas' should be the (only) child of the 'Window'.");
            Assert.That(_ellipses.Count, Is.EqualTo(5), "There have to be 5 Ellipses.");
            Assert.That(_ellipses.All(e => e.Parent == _canvas), Is.True, "The ellipses should be children of the 'Canvas'.");
        }

        [MonitoredTest("Should have 5 ellipses with the correct color and position"), Order(3)]
        public void _03_ShouldHave5EllipsesWithTheCorrectColorAndPosition()
        {
            Ellipse blueEllipse = _ellipses.FirstOrDefault(e => e.Stroke == Brushes.Blue);
            Assert.That(blueEllipse, Is.Not.Null, "Cannot find an ellipse with a 'Blue' stroke.");
            Ellipse blackEllipse = _ellipses.FirstOrDefault(e => e.Stroke == Brushes.Black);
            Assert.That(blackEllipse, Is.Not.Null, "Cannot find an ellipse with a 'Black' stroke.");
            Ellipse redEllipse = _ellipses.FirstOrDefault(e => e.Stroke == Brushes.Red);
            Assert.That(redEllipse, Is.Not.Null, "Cannot find an ellipse with a 'Red' stroke.");
            Ellipse yellowEllipse = _ellipses.FirstOrDefault(e => e.Stroke == Brushes.Yellow);
            Assert.That(yellowEllipse, Is.Not.Null, "Cannot find an ellipse with a 'Yellow' stroke.");
            Ellipse greenEllipse = _ellipses.FirstOrDefault(e => e.Stroke == Brushes.Green);
            Assert.That(greenEllipse, Is.Not.Null, "Cannot find an ellipse with a 'Green' stroke.");

            Assert.That(_ellipses.All(e => e.Width == 150), Is.True, "Not all ellipses have a Width of 150 pixels.");
            Assert.That(_ellipses.All(e => e.Height == 150), Is.True, "Not all ellipses have a Height of 150 pixels.");

            Assert.That(blueEllipse.GetValue(Canvas.TopProperty), Is.EqualTo(25), "The Blue Ellipse should be 25 pixels from the top.");
            Assert.That(blackEllipse.GetValue(Canvas.TopProperty), Is.EqualTo(25), "The Black Ellipse should be 25 pixels from the top.");
            Assert.That(redEllipse.GetValue(Canvas.TopProperty), Is.EqualTo(25), "The Red Ellipse should be 25 pixels from the top.");
            Assert.That(yellowEllipse.GetValue(Canvas.TopProperty), Is.EqualTo(75), "The Yellow Ellipse should be 75 pixels from the top.");
            Assert.That(greenEllipse.GetValue(Canvas.TopProperty), Is.EqualTo(75), "The Green Ellipse should be 75 pixels from the top.");

            Assert.That(blueEllipse.GetValue(Canvas.LeftProperty), Is.EqualTo(25), "The Blue Ellipse should be 25 pixels from the left.");
            Assert.That(blackEllipse.GetValue(Canvas.LeftProperty), Is.EqualTo(100), "The Black Ellipse should be 100 pixels from the left.");
            Assert.That(redEllipse.GetValue(Canvas.LeftProperty), Is.EqualTo(175), "The Red Ellipse should be 175 pixels from the left.");
            Assert.That(yellowEllipse.GetValue(Canvas.LeftProperty), Is.EqualTo(50), "The Yellow Ellipse should be 50 pixels from the left.");
            Assert.That(greenEllipse.GetValue(Canvas.LeftProperty), Is.EqualTo(125), "The Green Ellipse should be 125 pixels from the left.");

            Assert.That(blueEllipse.IsBehind(yellowEllipse, blackEllipse, greenEllipse), Is.True,
                "The blue ellipse should be behind yellow, black and green. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(yellowEllipse.IsBehind(blackEllipse, greenEllipse, redEllipse), Is.True,
                "The yellow ellipse should be behind black, green and red. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(blackEllipse.IsBehind(greenEllipse, redEllipse), Is.True,
                "The black ellipse should be behind green and red. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(greenEllipse.IsBehind(redEllipse), Is.True,
                "The green ellipse should be behind red. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");
        }
    }

    internal static class EllipseExtensions
    {
        public static bool IsBehind(this Ellipse ellipse, params Ellipse[] others)
        {
            int zIndex = (int)ellipse.GetValue(Canvas.ZIndexProperty);

            return others.All(other =>
            {
                int otherZIndex = (int)other.GetValue(Canvas.ZIndexProperty);
                return zIndex < otherZIndex;
            });
        }
    }
}