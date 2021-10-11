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
    [ExerciseTestFixture("dotnet2", "H03", "Exercise05", @"Exercise5\DockPanelWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class DockPanelWindowTests
    {
        private TestWindow<DockPanelWindow> _window;
        private DockPanel _dockPanel;
        private IList<Button> _buttons;
        private Image _image;
        private Button _topButton;
        private Button _rightButton;
        private Button _bottomButton;
        private Button _leftButton;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<DockPanelWindow>();
            _dockPanel = _window.GetUIElements<DockPanel>().FirstOrDefault();
            _image = _window.GetUIElements<Image>().FirstOrDefault();
            _buttons = _window.GetUIElements<Button>().ToList();
            _topButton = _buttons.FirstOrDefault(b => b.Content.ToString() == "Top");
            _rightButton = _buttons.FirstOrDefault(b => b.Content.ToString() == "Right");
            _bottomButton = _buttons.FirstOrDefault(b => b.Content.ToString() == "Bottom");
            _leftButton = _buttons.FirstOrDefault(b => b.Content.ToString() == "Left");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("DockPanel - The Window should contain a DockPanel"), Order(1)]
        public void _01_TheWindowShouldContainADockPanel()
        {
            Assert.That(_dockPanel, Is.Not.Null, "Window should contain a DockPanel");
            Assert.That(_dockPanel.Parent, Is.SameAs(_window.Window), "The parent element of the DockPanel is the Window");
        }

        [MonitoredTest("DockPanel - Should contain 4 Buttons And an Image"), Order(2)]
        public void _02_TheDockPanelShouldContain4ButtonsAndAnImage()
        {
            Assert.That(_buttons.Count, Is.EqualTo(4), "The DockPanel should contain 4 buttons");
            Assert.That(_image.Parent, Is.SameAs(_dockPanel), "The parent element of the Image is the DockPanel");
        }

        [MonitoredTest("DockPanel - The Buttons should have correct margins ans dimensions"), Order(3)]
        public void _03_TheButtonsShouldHaveCorrectMarginsAndDimensions()
        {
            Assert.That(_buttons.Count, Is.GreaterThan(0), "The DockPanel does not contain any buttons");
            Assert.That(_buttons.All(b => b.Height == 30), Is.True, "All buttons should have a Height of 30");
            Assert.That(_buttons.All(b => b.Width == 80), Is.True, "All buttons should have a Width of 80");
            Assert.That(_buttons.All(b => b.Margin.Left == 5), Is.True, "All buttons should have a Left Margin of 5");
            Assert.That(_buttons.All(b => b.Margin.Right == 5), Is.True, "All buttons should have a Right Margin of 5");
            Assert.That(_buttons.All(b => b.Margin.Top == 5), Is.True, "All buttons should have a Top Margin of 5");
            Assert.That(_buttons.All(b => b.Margin.Bottom == 5), Is.True, "All buttons should have a Bottom Margin of 5");
        }

        [MonitoredTest("DockPanel - All Buttons should have a the correct DockPanel.Dock Value"), Order(4)]
        public void _04_AllButtonsShouldHaveTheCorrectDockPanelDock()
        {
            Assert.That(_topButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Top), "The Top Button should have a 'Top' DockPanal.Dock Value");
            Assert.That(_leftButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Left), "The Left Button should have a 'Left' DockPanal.Dock Value");
            Assert.That(_rightButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Right), "The Right Button should have a 'Right' DockPanal.Dock Value");
            Assert.That(_bottomButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Bottom), "The Bottom Button should have a 'Bottom' DockPanal.Dock Value");
        }
    }
}

