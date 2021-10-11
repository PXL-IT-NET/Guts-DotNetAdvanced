using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;
using Guts.Client.Classic;

namespace Exercise5.Tests
{
    [ExerciseTestFixture("dotnet2", "H03", "Exercise05", @"Exercise5\WrapPanelWindow.xaml;Exercise5\WrapPanelWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class WrapPanelWindowTests
    {
        private TestWindow<WrapPanelWindow> _window;
        private Grid _grid;
        private StackPanel _stackPanel;
        private GroupBox _groupBox;
        private IList<RadioButton> _radioButtons;
        private WrapPanel _wrapPanel;
        private IList<Ellipse> _ellipses;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<WrapPanelWindow>();
            _grid = _window.GetUIElements<Grid>().FirstOrDefault();
            _groupBox = _grid.Children.OfType<GroupBox>().FirstOrDefault();
            _stackPanel = _window.GetUIElements<StackPanel>().FirstOrDefault();
            _wrapPanel = _window.GetUIElements<WrapPanel>().FirstOrDefault();
            _radioButtons = _window.GetUIElements<RadioButton>().ToList();
            _ellipses = _wrapPanel != null ? _wrapPanel.Children.OfType<Ellipse>().ToList() : new List<Ellipse>();

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("WrapPanel - Window should contain a Grid with 2 rows"), Order(1)]
        public void _01_WindowShouldContainAGridWith2Rows()
        {
            AssertGridHas2Cells();
        }

        private void AssertGridHas2Cells()
        {
            AssertHasOuterGrid();

            Assert.That(_grid.RowDefinitions, Has.Count.EqualTo(2), () => "The 'Grid' should have 2 rows defined.");

            Assert.That(_grid.RowDefinitions[0].Height.IsAuto, Is.True, "The first row of the outer grid should adjust to the height of its children.");
            Assert.That(_grid.ColumnDefinitions, Has.Count.EqualTo(0), () => "The 'Grid' should have no columns defined.");
        }

        private void AssertHasOuterGrid()
        {
            Assert.That(_grid, Is.Not.Null, "No 'Grid' could be found.");
            Assert.That(_grid.Parent, Is.SameAs(_window.Window),
                "The 'Grid' should be the child control of the 'Window'.");
        }

        [MonitoredTest("WrapPanel - First Row of Grid should contain a GroupBox"), Order(2)]
        public void _02_FirstRowOfGridShouldContainAGroupBox()
        {
            AssertGridHasGroupBoxInHisFirstRow();
        }

        private void AssertGridHasGroupBoxInHisFirstRow()
        {
            Assert.That(_groupBox, Is.Not.Null, "Grid should contain a StackPanel");
            Assert.That(_groupBox.GetValue(Grid.RowProperty), Is.EqualTo(0), "Grid should contain a StackPanel in its first row");
            Assert.That(_groupBox.Header, Is.EqualTo("Orientation"), "The header of the groupBox should be 'Orientation'");
        }


        [MonitoredTest("WrapPanel - The GroupBox contains a StackPanel with 2 radiobuttons"), Order(3)]
        public void _03_GroupBoxShouldContainAStackPanelWith2RadioButtons()
        {
            Assert.That(_stackPanel, Is.Not.Null, "There has to be a stackPanel on the window");
            Assert.That(_stackPanel.Parent, Is.EqualTo(_groupBox), "The StackPanel has to be within the GroupBox");
            Assert.That(_radioButtons.Count, Is.EqualTo(2), "The Groupbox has to contain 2 radioButtons");
            Assert.That(_radioButtons.All(r => r.Parent == _stackPanel), Is.True, "All radioButtons have to be inside the GroupBox");
        }

        [MonitoredTest("WrapPanel - The WrapPanel has to be in the second row of the Grid"), Order(4)]
        public void _04_HorizontalWrapPanelHasToBeInTheSecondRowOfTheGridAndContains8Ellipses()
        {
            Assert.That(_wrapPanel, Is.Not.Null, "There has to be a wrapPanel on the window");
            Assert.That(_wrapPanel.GetValue(Grid.RowProperty), Is.EqualTo(1), "Grid should contain a WrapPanel in its second row");
            Assert.That(_wrapPanel.Orientation, Is.EqualTo(Orientation.Horizontal), "The orientation of the WrapPanel has to be Horizontal");
            Assert.That(_radioButtons.Count, Is.EqualTo(2), "The GroupBox has to contain 2 radioButtons");
            Assert.That(_ellipses.All(r => r.Parent == _wrapPanel), Is.True, "All the ellipses should be within the wrapPanel");
            Assert.That(_ellipses.Count, Is.EqualTo(8), "There have to be 8 ellipses within the WrapPanel");
        }

        [MonitoredTest("WrapPanel - The orientation of the WrapPanel has to be vertical when clicking the Vertical RadioButton "), Order(5)]
        public void _05_TheOrientationOfTheWrapPanelHasToBecomeVerticalWhenClickingTheVerticalRadioButton()
        {
            RadioButton verticalRadioButton = _radioButtons.FirstOrDefault(r => r.Content.ToString() == "Vertical");
            Assert.That(verticalRadioButton, Is.Not.Null, "Cannot find a 'RadioButton' with content 'Vertical'.");
            verticalRadioButton.IsChecked = true;
            Assert.That(_wrapPanel.Orientation, Is.EqualTo(Orientation.Vertical), "The Orientation of the WrapPanel has to become Vertical when clicking the Vertical RadioButton");
        }

        [MonitoredTest("WrapPanel - The orientation of the WrapPanel has to be horizontal when clicking the Horizontal RadioButton "), Order(6)]
        public void _06_TheOrientationOfTheWrapPanelHasToBecomeHorizontalWhenClickingTheHorizontalRadioButton()
        {
            RadioButton horizontalRadioButton = _radioButtons.FirstOrDefault(r => r.Content.ToString() == "Horizontal");
            Assert.That(horizontalRadioButton, Is.Not.Null, "Cannot find a 'RadioButton' with content 'Horizontal'.");
            horizontalRadioButton.IsChecked = true;
            Assert.That(_wrapPanel.Orientation, Is.EqualTo(Orientation.Horizontal), "The Orientation of the WrapPanel has to become Horizontal when clicking the Horizontal RadioButton");
        }
    }
}

