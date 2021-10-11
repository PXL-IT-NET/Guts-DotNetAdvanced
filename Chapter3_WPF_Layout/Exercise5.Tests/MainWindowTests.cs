using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using NUnit.Framework;

namespace Exercise5.Tests
{
    [ExerciseTestFixture("dotnet2", "H03", "Exercise05", @"Exercise5\MainWindow.xaml;Exercise5\MainWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Grid _grid;
        private IList<Button> _allButtons;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            _grid = _window.GetUIElements<Grid>().FirstOrDefault();
            _allButtons = _window.GetUIElements<Button>().ToList();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        private bool VerifyClickEventHandler(object objectWithEvent, string eventName)
        {
            var eventStore = objectWithEvent.GetType()
            .GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(objectWithEvent, null);

            if (eventStore != null)
            {
                var clickEvent = ((RoutedEventHandlerInfo[])eventStore
                .GetType()
                .GetMethod("GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Invoke(eventStore, new object[] { Button.ClickEvent }))
                .First();
                return clickEvent.Handler.Method.Name == null ? false : true;

            }
            return false;
        }

        [MonitoredTest("MainWindow - Buttons should have Click event handlers"), Order(1)]
        public void _01_ButtonsShouldHaveClickEventHandlers()
        {
            Assert.That(_allButtons.Count, Is.EqualTo(4), "There should be 4 buttons");
            Assert.That(_allButtons.All(button => VerifyClickEventHandler(button, "Click")), Is.True, "All buttons should have a click event handler.");
        }

        [MonitoredTest("MainWindow - Should have a grid with 4 cells containing a Button"), Order(2)]
        public void _02_ShouldHaveAGridWith4CellsContainingAButton()
        {
            AssertGridHas4Cells();
            AssertCellsContainButtonsWithContent();
        }

        private void AssertCellsContainButtonsWithContent()
        {
            Assert.That(_allButtons.All(button => button.Content != null && (String)button.Content != String.Empty), Is.True, "All buttons should have a Content");

        }

        [MonitoredTest("MainWindow - The grid should have the correct element in each cell"), Order(3)]
        public void _03_TheGridShouldHaveTheCorrectElementInEachCell()
        {
            AssertGridHas4Cells();

            var allGridButtons = _grid.Children.OfType<Button>().ToList();
            AssertCellHasControl<Button>(0, 0);
            AssertCellHasControl<Button>(0, 1);
            AssertCellHasControl<Button>(1, 0);
            AssertCellHasControl<Button>(1, 1);
        }

        private void AssertHasOuterGrid()
        {
            Assert.That(_grid, Is.Not.Null, "No 'Grid' could be found.");
            Assert.That(_grid.Parent, Is.SameAs(_window.Window),
                "The 'Grid' should be the child control of the 'Window'.");
        }

        private void AssertCellHasControl<T>(int row, int column) where T : FrameworkElement
        {
            var control = _grid
                .FindVisualChildren<T>().FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column);

            var type = typeof(T);
            Assert.That(control, Is.Not.Null, $"No {type.Name} found in cell ({row},{column}).");
        }

        private void AssertGridHas4Cells()
        {
            AssertHasOuterGrid();

            Assert.That(_grid.RowDefinitions, Has.Count.EqualTo(2), () => "The 'Grid' should have 2 rows defined.");
            Assert.That(_grid.ColumnDefinitions, Has.Count.EqualTo(2), () => "The 'Grid' should have 2 columns defined.");
        }
    }
}
