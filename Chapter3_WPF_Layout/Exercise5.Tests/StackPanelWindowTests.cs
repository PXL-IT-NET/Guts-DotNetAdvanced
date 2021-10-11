using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using Guts.Client.Classic;

namespace Exercise5.Tests
{
    [ExerciseTestFixture("dotnet2", "H03", "Exercise05", @"Exercise5\StackPanelWindow.xaml;Exercise5\StackPanelWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class StackPanelWindowTests
    {
        private TestWindow<StackPanelWindow> _window;
        private Grid _grid;
        private GroupBox _groupBox;
        private StackPanel _orientationStackPanel, _stackPanel;
        private IList<RadioButton> _radioButtons;
        private IList<Button> _buttons;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<StackPanelWindow>();
            _grid = _window.GetUIElements<Grid>().FirstOrDefault();
            _groupBox = _grid.Children.OfType<GroupBox>().FirstOrDefault();
            _orientationStackPanel = _window.GetUIElements<StackPanel>().FirstOrDefault(s => s.Parent == _groupBox);
            _radioButtons = _window.GetUIElements<RadioButton>().ToList();
            _buttons = _window.GetUIElements<Button>().ToList();
            _stackPanel = _window.GetUIElements<StackPanel>().FirstOrDefault(s => s.Parent == _grid);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("StackPanel - Window should contain a Grid with 2 rows"), Order(1)]
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

        [MonitoredTest("StackPanel - First Row of Grid should contain a GroupBox"), Order(2)]
        public void _02_FirstRowOfGridShouldContainAGroupBox()
        {
            AssertGridHasGroupBoxInHisFirstRow();
        }

        private void AssertGridHasGroupBoxInHisFirstRow()
        {
            Assert.That(_groupBox, Is.Not.Null, "Grid should contain a GroupBox");
            Assert.That(_groupBox.GetValue(Grid.RowProperty), Is.EqualTo(0), "Grid should contain a GroupBox in its first row");
            Assert.That(_groupBox.Header, Is.EqualTo("Orientation"), "The header of the groupBox should be 'Orientation'");
        }

        [MonitoredTest("StackPanel - The GroupBox contains a StackPanel with 2 radiobuttons"), Order(3)]
        public void _03_GroupBoxShouldContainAStackPanelWith2RadioButtons()
        {
            Assert.That(_orientationStackPanel, Is.Not.Null, "There has to be a stackPanel on the window");
            Assert.That(_orientationStackPanel.Parent, Is.EqualTo(_groupBox), "The StackPanel has to be within the GroupBox");
            Assert.That(_radioButtons.Count, Is.EqualTo(2), "The StackPanel has to contain 2 radioButtons");
            Assert.That(_radioButtons.All(r => r.Parent == _orientationStackPanel), Is.True, "All radioButtons have to be inside the StackPanel");
        }

        [MonitoredTest("StackPanel - The StackPanel should be in the first row of the grid "), Order(4)]
        public void _04_TheStackPanelShouldBeInTheFirstRowOfTheGrid()
        {
            Assert.That(_stackPanel, Is.Not.Null, "The should be a stackPanel within the Grid");
            Assert.That(_stackPanel.GetValue(Grid.RowProperty), Is.EqualTo(1), "Grid should contain a StackPanel in its first row");
            Assert.That(_buttons.All(b => b.Parent == _stackPanel), Is.True, "The 2 Buttons should be inside the stackPanel");
            Assert.That(_buttons.Count, Is.EqualTo(2), "There should be 2 buttons inside the StackPanel");
        }

        [MonitoredTest("StackPanel - There should be an image with an image source on the first button "), Order(5)]
        public void _05_ThereShouldBeAnImageWithAnImageSourceOnTheFirstButton()
        {
            Image image = _buttons[0].Content as Image;
            Assert.That(image, Is.Not.Null, "The content of the first button should be an image");
            Assert.That(image.Source, Is.Not.Null, "The image source must be set correctly. " +
                                                   "Make sure the image is visible when you run the application. " +
                                                   "For this to work, the image must be present next the exe file (in the output directory) or be a resource of the assembly.");
        }
    }
}
