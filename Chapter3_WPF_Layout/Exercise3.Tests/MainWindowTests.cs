using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
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
        private IList<Rectangle> _rectangles;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            _canvas = _window.GetUIElements<Canvas>().FirstOrDefault();
            _ellipses = _canvas.FindVisualChildren<Ellipse>().ToList();
            _rectangles = _canvas.FindVisualChildren<Rectangle>().ToList();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
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

        [MonitoredTest("Should have 2 ellipses in a Canvas"), Order(2)]
        public void _02_ShouldHaveTwoEllipsesInACanvas()
        {
            Assert.That(_canvas, Is.Not.Null, "Cannot find a 'Canvas' in the window.");
            Assert.That(_canvas.Parent, Is.SameAs(_window.Window), "The 'Canvas' should be the (only) child of the 'Window'.");
            Assert.That(_ellipses.Count, Is.EqualTo(2), "There have to be 2 Ellipses.");
            Assert.That(_ellipses.All(e => e.Parent == _canvas), Is.True, "The ellipses should be children of the 'Canvas'.");
        }

        [MonitoredTest("Should have  rectangles in a Canvas"), Order(3)]
        public void _02_ShouldHaveSixRectanglesInACanvas()
        {
            Assert.That(_canvas, Is.Not.Null, "Cannot find a 'Canvas' in the window.");
            Assert.That(_canvas.Parent, Is.SameAs(_window.Window), "The 'Canvas' should be the (only) child of the 'Window'.");
            Assert.That(_rectangles.Count, Is.EqualTo(6), "There have to be 6 Rectangles.");
            Assert.That(_rectangles.All(e => e.Parent == _canvas), Is.True, "The rectangles should be children of the 'Canvas'.");
        }

        [MonitoredTest("Should have 2 ellipses and 6 rectangles with the correct color and position"), Order(3)]
        public void _03_ShouldHave2EllipsesAnd6RectanglesWithTheCorrectColorAndPosition()
        {
            IList<Ellipse> redEllipses = _ellipses.Where(e => e.Fill == Brushes.Red).OrderBy(e => e.GetValue(Canvas.TopProperty)).ToList();
            Assert.That(redEllipses, Has.Count.EqualTo(2), "There should be 2 red ellipses");
            IList<Rectangle> blueRectangles = _rectangles.Where(r => r.Fill == Brushes.Blue).OrderBy(e => e.GetValue(Canvas.TopProperty)).ToList();
            Assert.That(blueRectangles, Has.Count.EqualTo(2), "There should be 2 blue rectangles");
            IList<Rectangle> yellowRectangles = _rectangles.Where(r => r.Fill == Brushes.Yellow).OrderBy(e => e.GetValue(Canvas.TopProperty)).ToList();
            Assert.That(yellowRectangles, Has.Count.EqualTo(2), "There should be 2 yellow rectangles");
            IList<Rectangle> blackRectangles = _rectangles.Where(r => r.Fill == Brushes.Black).OrderBy(e => e.GetValue(Canvas.TopProperty)).ToList();
            Assert.That(blackRectangles, Has.Count.EqualTo(2), "There should be 2 black rectangles");

            AssertPosition(redEllipses.First(), "the first red ellipse", 80, 180);
            Assert.That(redEllipses.First().Width == 70, Is.True, "The first red ellipse should have a Width of 70.");
            Assert.That(redEllipses.First().Height == 70, Is.True, "The first red ellipse should have a Height of 70.");

            AssertPosition(blueRectangles.First(), "the first blue rectangle", 100, 100);
            AssertPosition(yellowRectangles.First(), "the first yellow rectangle", 150, 150);
            AssertPosition(blackRectangles.First(), "the first black rectangle", 200, 200);
            AssertPosition(blackRectangles.ElementAt(1), "the second black rectangle", 300, 200);
            AssertPosition(redEllipses.ElementAt(1), "the second red ellipse", 250, 100);
            Assert.That(redEllipses.ElementAt(1).Width == 150, Is.True, "The second red ellipse does not have a Width of 150 pixels.");
            Assert.That(redEllipses.ElementAt(1).Height == 150, Is.True, "The second red ellipse does not have a Height of 150 pixels.");

            AssertPosition(yellowRectangles.ElementAt(1), "the second yellow rectangle", 350, 150);
            AssertPosition(blueRectangles.ElementAt(1), "the second blue rectangle", 400, 100);

            Assert.That(_rectangles.All(e => e.Height == 100), Is.True, "Not all rectangles have a Height of 100 pixels.");
            Assert.That(_rectangles.All(e => e.Width == 100), Is.True, "Not all rectangles have a Width of 100 pixels.");

            Assert.That(blueRectangles.First().IsBehindEllipse(redEllipses.First()), Is.True,
                "The first blue rectangle should be behind the first red ellipse." +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(yellowRectangles.First().IsBehind(blueRectangles.First(), blackRectangles.First()), Is.True,
                "The first yellow rectangle should be behind the first blue and first black rectangle. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(blackRectangles.First().GetValue(Canvas.ZIndexProperty), Is.EqualTo(blackRectangles.ElementAt(1).GetValue(Canvas.ZIndexProperty)),
                "The two black rectangles should not be behind each other. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(redEllipses.ElementAt(1).IsBehindRectangle(blackRectangles.First(), blackRectangles.ElementAt(1), yellowRectangles.ElementAt(1)), Is.True,
                "The second red ellipse should not be behind the first black rectangle. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(blackRectangles.ElementAt(1).IsBehind(yellowRectangles.ElementAt(1)), Is.True,
                "The second blue rectangle should be behind the second yellow rectangle. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");
        }

        private void AssertPosition(FrameworkElement element, string elementDescription, int expectedTopMargin, int expectedLeftMargin)
        {
            Assert.That(element, Is.Not.Null, $"Cannot find {elementDescription}.");
            double canvasTop = Convert.ToDouble(element.GetValue(Canvas.TopProperty));
            Assert.That(canvasTop, Is.EqualTo(expectedTopMargin),
                $"An attached property should be used to set the distance between {elementDescription} and the top of the Canvas to {expectedTopMargin}.");
            double canvasLeft = Convert.ToDouble(element.GetValue(Canvas.LeftProperty));
            Assert.That(canvasLeft, Is.EqualTo(expectedLeftMargin),
                $"An attached property should be used to set the distance between {elementDescription} and the left of the Canvas to {expectedLeftMargin}.");
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

        public static bool IsBehindRectangle(this Ellipse ellipse, params Rectangle[] others)
        {
            int zIndex = (int)ellipse.GetValue(Canvas.ZIndexProperty);

            return others.All(other =>
            {
                int otherZIndex = (int)other.GetValue(Canvas.ZIndexProperty);
                return zIndex < otherZIndex;
            });
        }
    }

    internal static class RectangleExtensions
    {
        public static bool IsBehind(this Rectangle rectangle, params Rectangle[] others)
        {
            int zIndex = (int)rectangle.GetValue(Canvas.ZIndexProperty);

            return others.All(other =>
            {
                int otherZIndex = (int)other.GetValue(Canvas.ZIndexProperty);
                return zIndex < otherZIndex;
            });
        }
    
        public static bool IsBehindEllipse(this Rectangle rectangle, params Ellipse[] others)
        {
            int zIndex = (int)rectangle.GetValue(Canvas.ZIndexProperty);

            return others.All(other =>
            {
                int otherZIndex = (int)other.GetValue(Canvas.ZIndexProperty);
                return zIndex < otherZIndex;
            });
        }
    }
}