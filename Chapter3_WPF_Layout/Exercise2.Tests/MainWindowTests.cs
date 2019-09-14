using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    [ExerciseTestFixture("dotnet2", "H03", "Exercise02", 
        @"Exercise2\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Grid _grid;
        private DockPanel _dockPanel;
        private WrapPanel _wrapPanel;
        private StackPanel _stackPanel;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            _grid = _window.GetUIElements<Grid>().FirstOrDefault();
            _dockPanel = _window.GetUIElements<DockPanel>().FirstOrDefault();
            _stackPanel = _window.GetUIElements<StackPanel>().FirstOrDefault();
            _wrapPanel = _window.GetUIElements<WrapPanel>().FirstOrDefault();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _01_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise2\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("45-87-48-FC-AD-0E-8A-18-73-4F-C9-8C-27-6A-7A-D3"),
                () =>
                    $"The file '{codeBehindFilePath}' has changed. " +
                    "Undo your changes on the file to make this test pass. " +
                    "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("All Buttons should have some margin"), Order(2)]
        public void _02_AllButtonsShouldHaveSomeMargin()
        {
            AssertHasOuterGrid();
            var allButtons = _grid.FindVisualChildren<Button>().ToList();

            Assert.That(allButtons,
                Has.All.Matches((Button button) => HasMargin(button)),
                "All 'Button' elements in the XAML should have some margin on all sides. " +
                "E.g. 'Margin=\"4\"'");
        }

        [MonitoredTest("All TextBlocks should have some margin and a background"), Order(3)]
        public void _03_AllTextBlocksShouldHaveSomeMarginAndABackground()
        {
            AssertHasOuterGrid();
            var allTextBlocks = _grid.FindVisualChildren<TextBlock>().Where(tb => tb.TemplatedParent == null).ToList();

            Assert.That(allTextBlocks,
                Has.All.Matches((TextBlock textBlock) => HasMargin(textBlock)),
                "All 'TextBlock' elements in the XAML should have some margin on all sides. " +
                "E.g. 'Margin=\"4\"'");

            Assert.That(allTextBlocks,
                Has.All.Matches((TextBlock textBlock) => HasNoneWhiteBackground(textBlock)),
                "All 'TextBlock' elements in the XAML should have a solid non-white background color. ");
        }

        [MonitoredTest("None of the Buttons should be aligned to the left or the top"), Order(4)]
        public void _04_NoneOfTheButtonsShouldBeAlignedToTheLeftOrTheTop()
        {
            AssertHasOuterGrid();
            var allButtons = _grid.FindVisualChildren<Button>().ToList();

            Assert.That(allButtons,
                Has.None.Matches((Button button) => button.HorizontalAlignment == HorizontalAlignment.Left),
                () => "There is at least one 'Button' that has 'HorizontalAlignment' left.");

            Assert.That(allButtons,
                Has.None.Matches((Button button) => button.VerticalAlignment == VerticalAlignment.Top),
                () => "There is at least one 'Button' that has 'VerticalAlignment' top.");
        }

        [MonitoredTest("None of the TextBlocks should be aligned to the left or the top"), Order(5)]
        public void _05_NoneOfTheTextBlocksShouldBeAlignedToTheLeftOrTheTop()
        {
            AssertHasOuterGrid();
            var allTextBlocks = _grid.FindVisualChildren<TextBlock>().Where(tb => tb.TemplatedParent == null).ToList();

            Assert.That(allTextBlocks,
                Has.None.Matches((TextBlock tb) => tb.HorizontalAlignment == HorizontalAlignment.Left),
                () => "There is at least one 'TextBlock' that has 'HorizontalAlignment' left.");

            Assert.That(allTextBlocks,
                Has.None.Matches((TextBlock tb) => tb.VerticalAlignment == VerticalAlignment.Top),
                () => "There is at least one 'TextBlock' that has 'VerticalAlignment' top.");
        }

        [MonitoredTest("Should have a grid with 6 cells arranged correctly"), Order(6)]
        public void _06_ShouldHaveAGridWith6CellsArrangedCorrectly()
        {
            AssertGridHas6Cells();

            var row1Definition = _grid.RowDefinitions[0];
            var row2Definition = _grid.RowDefinitions[1];
            var row3Definition = _grid.RowDefinitions[2];

            Assert.That(row1Definition.Height.IsAuto, Is.True,
                "The height of the first row of the 'Grid' should adjust to the height of its children.");

            Assert.That(row2Definition.Height.IsStar && row3Definition.Height.IsStar, Is.True,
                "The second and third rows of the 'Grid' should adjust dynamically to the height of the 'Window'.");

            Assert.That(row2Definition.ActualHeight * 2, Is.EqualTo(row3Definition.ActualHeight).Within(1.0),
                "The third row should always be 2 times higher than the second row.");

            var column1Definition = _grid.ColumnDefinitions[0];
            var column2Definition = _grid.ColumnDefinitions[1];
            Assert.That(column1Definition.Width.IsAbsolute, Is.True,
                "The first column of the 'Grid' should have an absolute width (of e.g. 100).");
            Assert.That(column2Definition.Width.IsStar, Is.True,
                "The second column of the 'Grid' should take up the remaining space.");
        }

        [MonitoredTest("The grid should have the correct element in each cell"), Order(7)]
        public void _07_TheGridShouldHaveTheCorrectElementInEachCell()
        {
            AssertGridHas6Cells();

            var allGridButtons = _grid.Children.OfType<Button>().ToList();
            AssertCellHasControl<Button>(0, 0);
            AssertCellHasControl<StackPanel>(0, 1);
            AssertCellHasControl<Button>(1, 0);
            AssertCellHasControl<WrapPanel>(1, 1);
            AssertCellHasControl<Button>(2, 0);
            AssertCellHasControl<DockPanel>(2, 1);
        }

        [MonitoredTest("Should have a StackPanel of Buttons in the top right cell"), Order(8)]
        public void _08_ShouldHaveAStackPanelOfButtonsInTheTopRightCell()
        {
            AssertHasOuterGrid();
            Assert.That(_stackPanel, Is.Not.Null, () => "No 'StackPanel' could be found.");
            Assert.That(_stackPanel.Parent, Is.SameAs(_grid),
                () => "The 'StackPanel' should be a child element (content) of the 'Grid'.");

            Assert.That(_stackPanel.Orientation, Is.EqualTo(Orientation.Vertical),
                "The 'StackPanel' should have a vertical orientation.");

            var buttons = _stackPanel.Children.OfType<Button>().ToList();
            Assert.That(buttons, Has.Count.GreaterThanOrEqualTo(2),
                "There should be at least 2 'Button' elements inside the 'StackPanel'.");
        }

        [MonitoredTest("Should have a WrapPanel with multiple TextBlocks"), Order(9)]
        public void _09_ShouldHaveAWrapPanelWithMultipleTextBlocks()
        {
            AssertHasOuterGrid();
            Assert.That(_wrapPanel, Is.Not.Null, "No 'WrapPanel' could be found.");
            Assert.That(_wrapPanel.Parent, Is.SameAs(_grid),
                () => "The 'WrapPanel' should be a child element of the 'Grid'.");

            var textBlocks = _wrapPanel.Children.OfType<TextBlock>().ToList();
            Assert.That(textBlocks, Has.Count.GreaterThanOrEqualTo(2), 
                "There must be at least 2 'TextBlocks' in the 'WrapPanel'.");

            Assert.That(textBlocks, Has.None.Matches((TextBlock tb) => string.IsNullOrEmpty(tb.Text) || tb.Text.Length < 10), 
                "All the 'TextBlocks' should have a 'Text' of at least 10 characters.");
        }

        [MonitoredTest("Should have a DockPanel with correctly docked TextBlocks"), Order(10)]
        public void _10_ShouldHaveADockPanelWithCorrectlyDockedTextBlocks()
        {
            AssertHasOuterGrid();
            Assert.That(_dockPanel, Is.Not.Null, "No 'DockPanel' could be found.");
            Assert.That(_dockPanel.Parent, Is.SameAs(_grid),
                () => "The 'DockPanel' should be a child element of the 'Grid'.");

            var textBlocks = _dockPanel.Children.OfType<TextBlock>().ToList();
            Assert.That(textBlocks, Has.Count.EqualTo(5), "The 'DockPanel' should have 5 'TextBlocks'.");

            Assert.That(textBlocks, Has.None.Matches((TextBlock tb) => string.IsNullOrEmpty(tb.Text) || tb.Text.Length < 10),
                "All the 'TextBlocks' should have a 'Text' of at least 10 characters.");

            AssertIsDocked(textBlocks[0], Dock.Top);
            AssertIsDocked(textBlocks[1], Dock.Left);
            AssertIsDocked(textBlocks[2], Dock.Bottom);
            AssertIsDocked(textBlocks[3], Dock.Right);
            AssertIsDocked(textBlocks[4], Dock.Right);
        }

        private void AssertHasOuterGrid()
        {
            Assert.That(_grid, Is.Not.Null, "No 'Grid' could be found.");
            Assert.That(_grid.Parent, Is.SameAs(_window.Window),
                "The 'Grid' should be the child control of the 'Window'.");
        }

        private bool HasNoneWhiteBackground(TextBlock textBlock)
        {
            var solidColorBrush = textBlock.Background as SolidColorBrush;
            if (solidColorBrush == null) return false;
            if (solidColorBrush.Color == Colors.White) return false;
            return true;
        }

        private void AssertCellHasControl<T>(int row, int column) where T: FrameworkElement
        {
            var control = _grid
                .FindVisualChildren<T>().FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column);

            var type = typeof(T);
            Assert.That(control, Is.Not.Null, $"No {type.Name} found in cell ({row},{column}).");
        }

        private bool HasMargin(FrameworkElement element)
        {
            return element.Margin.Left > 0 &&
                   element.Margin.Top > 0 &&
                   element.Margin.Right > 0 &&
                   element.Margin.Bottom > 0;
        }

        private bool HasPadding(Button button)
        {
            return button.Padding.Left > 0 &&
                   button.Padding.Top > 0 &&
                   button.Padding.Right > 0 &&
                   button.Padding.Bottom > 0;
        }

        private void AssertGridHas6Cells()
        {
            AssertHasOuterGrid();

            Assert.That(_grid.RowDefinitions, Has.Count.EqualTo(3), () => "The 'Grid' should have 3 rows defined.");
            Assert.That(_grid.ColumnDefinitions, Has.Count.EqualTo(2), () => "The 'Grid' should have 2 columns defined.");
        }

        private void AssertIsDocked(TextBlock textBlock, Dock expectedDock)
        {
            Assert.That(DockPanel.GetDock(textBlock), Is.EqualTo(expectedDock),
                $"One of the 'TextBlocks' is not docked correctly. Expected 'Dock': '{expectedDock}'.");
        }

    }
}
