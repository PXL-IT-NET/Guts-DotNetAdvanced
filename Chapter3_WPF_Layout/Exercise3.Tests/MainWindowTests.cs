using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        private IList<Ellipse> _ellipsesList;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            _canvas = _window.GetUIElements<Canvas>().FirstOrDefault();

            _ellipsesList = _canvas.FindVisualChildren<Ellipse>().ToList();

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
            Assert.That(fileHash, Is.EqualTo("90-4F-41-EA-49-EC-42-AD-F0-69-F1-ED-DE-4E-16-97"),
                () =>
                    $"The file '{codeBehindFilePath}' has changed. " +
                    "Undo your changes on the file to make this test pass. " +
                    "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("There have to be 5 ellipses"), Order(2)]
        public void _02_ShouldHaveFiveEllipses()
        {
            Assert.That(_ellipsesList.Count, Is.EqualTo(5), "There have to be 5 Ellipses.");
        }

        [MonitoredTest("There have to be 5 ellipses"), Order(2)]
        public void _03_ShouldHave5EllipsesWithTheCorrectColorAndPosition()
        {
            var blueEllipse = _ellipsesList.FirstOrDefault(e => e.Stroke == Brushes.Blue);
            var blackEllipse = _ellipsesList.FirstOrDefault(e => e.Stroke == Brushes.Black);
            var redEllipse = _ellipsesList.FirstOrDefault(e => e.Stroke == Brushes.Red);
            var yellowEllipse = _ellipsesList.FirstOrDefault(e => e.Stroke == Brushes.Yellow);
            var greenEllipse = _ellipsesList.FirstOrDefault(e => e.Stroke == Brushes.Green);


            Assert.That(blueEllipse.GetValue(Canvas.ZIndexProperty), Is.EqualTo(1), "The Blue Ellipse should be behind all the other ellipses.");
            Assert.That(yellowEllipse.GetValue(Canvas.ZIndexProperty), Is.EqualTo(2), "The Yellow Ellipse should be between the Blue and the Black ellipse.");
            Assert.That(blackEllipse.GetValue(Canvas.ZIndexProperty), Is.EqualTo(3), "The Black Ellipse should be between the Yellow and the Green ellipse.");
            Assert.That(greenEllipse.GetValue(Canvas.ZIndexProperty), Is.EqualTo(4), "The Green Ellipse should be between the Black and the Red ellipse.");
            Assert.That(redEllipse.GetValue(Canvas.ZIndexProperty), Is.EqualTo(5), "The Red Ellipse should be before all the other ellipses.");

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

            Assert.That(_ellipsesList.All(e => e.Width == 150), Is.True, "Not all ellipses have a Width of 150 pixels.");
            Assert.That(_ellipsesList.All(e => e.Height == 150), Is.True, "Not all ellipses have a Height of 150 pixels.");

        }



    }
}
