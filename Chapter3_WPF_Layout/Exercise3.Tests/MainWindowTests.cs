using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            Ellipse firstRedEllipse = _ellipses.FirstOrDefault(e => e.Fill == Brushes.Red && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 80);
            Assert.That(firstRedEllipse, Is.Not.Null, "Cannot find an ellipse with a 'Red' fill and a top margin o 80 pixels.");
            Assert.That(firstRedEllipse.Width == 70, Is.True, "The first red ellipse does not have a Width of 70 pixels.");
            Assert.That(firstRedEllipse.Height == 70, Is.True, "The first red ellipse does not have a Height of 70 pixels.");

            Rectangle firstBlueRectangle = _rectangles.FirstOrDefault(e => e.Fill == Brushes.Blue && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 100);
            Assert.That(firstBlueRectangle, Is.Not.Null, "Cannot find a 'Blue' rectangle with a top margin of 100 pixels.");

            Rectangle firstYellowRectangle = _rectangles.FirstOrDefault(e => e.Fill == Brushes.Yellow && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 150);
            Assert.That(firstYellowRectangle, Is.Not.Null, "Cannot find a 'Yellow' rectangle with a top margin of 150 pixels.");

            Rectangle firstBlackRectangle = _rectangles.FirstOrDefault(e => e.Fill == Brushes.Black && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 200);
            Assert.That(firstBlueRectangle, Is.Not.Null, "Cannot find a 'Black' rectangle with a top margin of 200 pixels.");

            Rectangle secondBlackRectangle = _rectangles.FirstOrDefault(e => e.Fill == Brushes.Black && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 300);
            Assert.That(secondBlackRectangle, Is.Not.Null, "Cannot find a 'Black' rectangle with a top margin of 300 pixels.");

            Ellipse secondRedEllipse = _ellipses.FirstOrDefault(e => e.Fill == Brushes.Red && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 250);
            Assert.That(secondRedEllipse, Is.Not.Null, "Cannot find an ellipse with a 'Red' fill and a top margin of 250.");
            Assert.That(secondRedEllipse.Width == 150, Is.True, "The second red ellipse does not have a Width of 150 pixels.");
            Assert.That(secondRedEllipse.Height == 150, Is.True, "The second red ellipse does not have a Height of 150 pixels.");

            Rectangle secondYellowRectangle = _rectangles.FirstOrDefault(e => e.Fill == Brushes.Yellow && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 350);
            Assert.That(secondYellowRectangle, Is.Not.Null, "Cannot find a 'Yellow' rectangle with a top margin of 350 pixels.");

            Rectangle secondBlueRectangle = _rectangles.FirstOrDefault(e => e.Fill == Brushes.Blue && Convert.ToInt32(e.GetValue(Canvas.TopProperty)) == 400);
            Assert.That(secondBlueRectangle, Is.Not.Null, "Cannot find a 'Blue' rectangle with a top margin of 400 pixels.");

            Assert.That(_rectangles.All(e => e.Height == 100), Is.True, "Not all rectangles have a Height of 100 pixels.");
            Assert.That(_rectangles.All(e => e.Width == 100), Is.True, "Not all rectangles have a Width of 100 pixels.");
        

            Assert.That(firstRedEllipse.GetValue(Canvas.LeftProperty), Is.EqualTo(180), "The first Red Ellipse should be 180 pixels from the left.");
            Assert.That(firstBlueRectangle.GetValue(Canvas.LeftProperty), Is.EqualTo(100), "The first Blue Rectangle should be 100 pixels from the left.");
            Assert.That(firstYellowRectangle.GetValue(Canvas.LeftProperty), Is.EqualTo(150), "The First Yellow Rectangle should be 150 pixels from the left.");
            Assert.That(firstBlackRectangle.GetValue(Canvas.LeftProperty), Is.EqualTo(200), "The first Black Rectangle should be 200 pixels from the left.");
            Assert.That(secondBlackRectangle.GetValue(Canvas.LeftProperty), Is.EqualTo(200), "The second Black Rectangle should be 200 pixels from the left.");
            Assert.That(secondYellowRectangle.GetValue(Canvas.LeftProperty), Is.EqualTo(150), "The second Yellow Rectangle should be 150 pixels from the left.");
            Assert.That(secondBlueRectangle.GetValue(Canvas.LeftProperty), Is.EqualTo(100), "The second Blue Rectangle should be 100 pixels from the left.");

            Assert.That(firstBlueRectangle.IsBehindEllipse(firstRedEllipse), Is.True, 
                "The first blue rectangle should be behind the first red ellipse." +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");
                
            Assert.That(firstYellowRectangle.IsBehind(firstBlueRectangle, firstBlackRectangle), Is.True,
                "The first yellow rectangle should be behind the first blue rectangle. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(firstBlackRectangle.GetValue(Canvas.ZIndexProperty), Is.EqualTo(secondBlackRectangle.GetValue(Canvas.ZIndexProperty)),
                "The two black rectangles should not be behind each other. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(secondRedEllipse.IsBehindRectangle(firstBlackRectangle, secondBlackRectangle, secondYellowRectangle), Is.True,
                "The second red ellipse should not be behind the first black rectangle. " +
                "Tip: make use of an attached property 'ZIndex' of 'Canvas'.");

            Assert.That(secondBlueRectangle.IsBehind(secondYellowRectangle), Is.True,
                "The second blue rectangle should be behind the second yellow rectangle. " +
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