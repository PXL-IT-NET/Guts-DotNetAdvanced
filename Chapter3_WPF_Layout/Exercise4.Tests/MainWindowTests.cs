using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise4.Tests
{
    [ExerciseTestFixture("dotnet2", "H03", "Exercise04", @"Exercise4\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Grid _grid;
        private IList<Label> _labels;
        private IList<GridSplitter> _gridSplitters;


        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            _grid = _window.GetUIElements<Grid>().FirstOrDefault();
            _labels = _window.GetUIElements<Label>().ToList();
            _gridSplitters = _window.GetUIElements<GridSplitter>().ToList();
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
            var codeBehindFilePath = @"Exercise4\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("90-4F-41-EA-49-EC-42-AD-F0-69-F1-ED-DE-4E-16-97"),
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass. " +
                "This exercise can be completed by purely working with XAML.");
        }


        [MonitoredTest("Grid should have 5 columns"), Order(2)]
        public void _02_GridShouldHaveFiveColumns()
        {
            Assert.That(_grid.ColumnDefinitions.Count, Is.EqualTo(5), "There have to be 5 columns (3 labels and 2 grid splitters).");
        }

        [MonitoredTest("Should have 3 labels with the correct color"), Order(3)]
        public void _03_ShouldHaveThreeLabelsWithTheCorrectColor()
        {
            Assert.That(_labels.Count, Is.EqualTo(3), () => "The grid must contain 3 labels");
            Assert.That(_labels.All(l => l.Parent == _grid), Is.True, "All labels must be direct children of the grid");
            Assert.That(_labels[0].Background == Brushes.Black, Is.True, "The first label should be black");
            Assert.That(_labels[1].Background == Brushes.Yellow, Is.True, "The second label should be yellow");
            Assert.That(_labels[2].Background == Brushes.Red, Is.True, "The third label should be red");
        }

        [MonitoredTest("Labels should be in the correct grid column"), Order(4)]
        public void _04_ShouldHaveLabelsInTheCorrectGridColumn()
        {
            Assert.That(_labels.Count, Is.EqualTo(3), () => "The grid must contain 3 labels");
            Assert.That(_labels[0].GetValue(Grid.ColumnProperty), Is.EqualTo(0), () => "The first label should be in the first column");
            Assert.That(_labels[1].GetValue(Grid.ColumnProperty), Is.EqualTo(2), () => "The second label should be in the third column");
            Assert.That(_labels[2].GetValue(Grid.ColumnProperty), Is.EqualTo(4), () => "The third label should be in the last column");
        }

        [MonitoredTest("There have to be 2 GridSplitters on the left and the right of the middle column"), Order(5)]
        public void _05_ShouldHaveTwoGridSplittersLeftAndRightOfTheMiddleColumn()
        {
            Assert.That(_gridSplitters.Count, Is.EqualTo(2), () => "There should be two GridSplitters.");

            Assert.That(_gridSplitters,
                Has.One.Matches((GridSplitter splitter) => (int)splitter.GetValue(Grid.ColumnProperty) == 1),
                () => "There should be one GridSplitter at the left of the middle column");

            Assert.That(_gridSplitters,
                Has.One.Matches((GridSplitter splitter) => (int)splitter.GetValue(Grid.ColumnProperty) == 3),
                () => "There should be one GridSplitter at the right of the middle column");
        }

        [MonitoredTest("Should have horizontal and vertical alignment set correctly for the splitters"), Order(6)]
        public void _06_ShouldHaveHorizontalAndVerticalAlignmentSetCorrectlyForTheSplitters()
        {
            Assert.That(_gridSplitters.Count, Is.EqualTo(2), () => "There should be two GridSplitters.");

            Assert.That(_gridSplitters,
                Has.All.Matches((GridSplitter splitter) => splitter.HorizontalAlignment == HorizontalAlignment.Center),
                "Not all splitters are horizontally centered");

            Assert.That(_gridSplitters,
                Has.All.Matches((GridSplitter splitter) => splitter.VerticalAlignment == VerticalAlignment.Stretch),
                "Not all splitters are vertically stretched");
        }

        [MonitoredTest("Should have the window as the scope for size sharing"), Order(7)]
        public void _07_ShouldHaveTheWindowAsTheScopeForSizeSharing()
        {
            Assert.That(_window.Window.GetValue(Grid.IsSharedSizeScopeProperty), Is.True,
                "The property 'IsSharedSizeScope' of 'Grid' must be true for 'Window'. " +
                "See https://docs.microsoft.com/dotnet/api/system.windows.controls.grid.issharedsizescope");
        }

        [MonitoredTest("The first and the last column have to share the same size group"), Order(8)]
        public void _08_ShouldHaveTheFirstAndTheLastColumnInTheSameSizeGroup()
        {
            _02_GridShouldHaveFiveColumns();

            Assert.That(_grid.ColumnDefinitions.First().SharedSizeGroup, Is.Not.Null.Or.Empty,
                "There first column should have a 'SharedSizeGroup' defined. " +
                "See https://docs.microsoft.com/dotnet/desktop/wpf/controls/how-to-share-sizing-properties-between-grids?view=netframeworkdesktop-4.8");
            Assert.That(_grid.ColumnDefinitions.Last().SharedSizeGroup, Is.Not.Null.Or.Empty,
                "There last column should have a 'SharedSizeGroup' defined. " +
                "See https://docs.microsoft.com/dotnet/desktop/wpf/controls/how-to-share-sizing-properties-between-grids?view=netframeworkdesktop-4.8");
            Assert.That(_grid.ColumnDefinitions.First().SharedSizeGroup,
                Is.EqualTo(_grid.ColumnDefinitions.Last().SharedSizeGroup),
                "The first and the last column have to share the same Size Group.");
        }

        [MonitoredTest("Should have auto width colums except for the middle column"), Order(9)]
        public void _09_ShouldHaveAutomWidthColumnsExceptForTheMiddleColumn()
        {
            _02_GridShouldHaveFiveColumns();

            var columns = new List<ColumnDefinition>(_grid.ColumnDefinitions);
            if (columns.Count > 2)
            {
                columns.RemoveAt(2);
            }

            Assert.That(columns,
                Has.All.Matches((ColumnDefinition column) => HasAutoColumnWidth(column)),
                () => "All (except the middle) 'ColumnDefinition' elements in the XAML should have auto Width. " +
                      "E.g. 'Width=\"auto\"'");
        }

        private bool HasAutoColumnWidth(ColumnDefinition columnDefinition)
        {
            return columnDefinition.Width == GridLength.Auto;
        }
    }
}