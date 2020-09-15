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
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _01_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise4\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("45-87-48-FC-AD-0E-8A-18-73-4F-C9-8C-27-6A-7A-D3"),
                () =>
                    $"The file '{codeBehindFilePath}' has changed. " +
                    "Undo your changes on the file to make this test pass. " +
                    "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("There have to columns participating In Size Sharing"), Order(2)]
        public void _02_ShouldHaveTwoColumnsParticipatingInSizeSharing()
        {
            Assert.That(_grid.GetValue(Grid.IsSharedSizeScopeProperty), Is.True, "There have to be 5 Ellipses.");
        }

        [MonitoredTest("There have to columns participating In Size Sharing"), Order(3)]
        public void _03_ShouldHaveFiveColumnsDefinitions()
        {
            Assert.That(_grid.ColumnDefinitions.Count, Is.EqualTo(5), "There have to be 5 columns.");
        }

        [MonitoredTest("The first and the last column have to share the same Size group"), Order(4)]
        public void _04_ShouldHaveTheFirstAndTheLastColumnInTheSameSizeGroup()
        {
            Assert.That(_grid.ColumnDefinitions[0].SharedSizeGroup, Is.EqualTo(_grid.ColumnDefinitions[4].SharedSizeGroup), "There first and the last column have to share the samen Size Group.");
        }
        [MonitoredTest("All the columns is given as much width as the elements within require, except for the middle (3rd) column" ), Order(5)]
        public void _05_ShouldHaveAutomaticWidthColumnsExceptForTheMiddleColumn()
        {
            ColumnDefinitionCollection columns = _grid.ColumnDefinitions;
            columns.RemoveAt(2);
            Assert.That(columns,
                Has.All.Matches((ColumnDefinition column) => HasAutoColumnWidth(column)),
                () => "All 'ColumnDefinition elements in the XAML should have auto Width. " +
                      "E.g. 'Width=\"auto\"'");
        }

        private bool HasAutoColumnWidth(ColumnDefinition columnDefinition)
        {
            return columnDefinition.Width == GridLength.Auto;
        }

        [MonitoredTest("The first and the last column have to share the same Size group"), Order(6)]
        public void _6_ShouldHaveThreeLabels()
        {
            Assert.That(_labels.Count(), Is.EqualTo(3), () => "The grid must contain 3 labels");
        }

        [MonitoredTest("The first and the last column have to share the same Size group"), Order(7)]
        public void _7_ShouldHaveThreeLabels()
        {            
            Assert.That(_labels.Count(), Is.EqualTo(3), () => "The grid must contain 3 labels");
            Assert.That(_labels[0].Background == Brushes.Black, Is.True, () => "The first label should be black");
            Assert.That(_labels[1].Background == Brushes.Yellow, Is.True, () => "The second label should be black");
            Assert.That(_labels[2].Background == Brushes.Red, Is.True, () => "The third label should be black");
        }

        [MonitoredTest("Labels should be in the correct grid column"), Order(8)]
        public void _8_ShouldHaveLabelsInTheCorrectGridColumn()
        {
            Assert.That(_labels[0].GetValue(Grid.ColumnProperty), Is.EqualTo(0), () => "The first label should be in the first column");
            Assert.That(_labels[1].GetValue(Grid.ColumnProperty), Is.EqualTo(2), () => "The second label should be in the third column");
            Assert.That(_labels[2].GetValue(Grid.ColumnProperty), Is.EqualTo(4), () => "The third label should be in the last column");
        }

        [MonitoredTest("Labels should be in the correct grid column"), Order(9)]
        public void _9_ShouldHaveTwoGridSplitters()
        {
            Assert.That(_gridSplitters.Count, Is.EqualTo(2), () => "There should be two GridSplitters (Left and Right from the Yellow column).");
        }

        [MonitoredTest("There have to be Two GridSplitters on the Left and the Right of the middle column"), Order(9)]
        public void _10_ShouldHaveTwoGridSplittersLeftAndRightFromTheMiddleColumn()
        {
            Assert.That(_gridSplitters,
                Has.One.Matches((GridSplitter splitter) => (int)splitter.GetValue(Grid.ColumnProperty)==1), 
                () => "There should be one GridSplitter at the left of column 1");
            
            Assert.That(_gridSplitters,
                Has.One.Matches((GridSplitter splitter) => (int)splitter.GetValue(Grid.ColumnProperty) == 3),
                () => "There should be one GridSplitter at the left of column 3");



        }

        [MonitoredTest("Horizontal Alignement of the GridSplitter Should be Center and Vertical Alignement Should be Stretch"), Order(10)]
        public void _10_ShouldHaveHorizontalAndVerticalAlignementSetForTheGridSplitter()
        {
            Assert.That(_gridSplitters,
                Has.All.Matches((GridSplitter splitter) => splitter.HorizontalAlignment == HorizontalAlignment.Center),
                () => "There should be one GridSplitter at the left of column 1");

            Assert.That(_gridSplitters,
                Has.All.Matches((GridSplitter splitter) => splitter.VerticalAlignment == VerticalAlignment.Stretch),
                () => "There should be one GridSplitter at the left of column 1");

        }


    }
}
