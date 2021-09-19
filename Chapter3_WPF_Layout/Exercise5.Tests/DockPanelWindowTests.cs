using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Exercise5.Tests
{
    //[ExerciseTestFixture("dotnet2", "H03", "Exercise05", @"Exercise5\DockPanelWindow.xaml")]
    [TestFixture]
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
            //_buttons = _dockPanel.Children.OfType<Button>().ToList();
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
            Dispatcher.CurrentDispatcher.InvokeShutdown();
            _window.Dispose();
        }

        [MonitoredTest("The Window Should contain a DockPanel"), Order(1)]
        public void _01_TheWindowShouldContainADockPanel()
        {
            Assert.That(_dockPanel, Is.Not.Null, "Window should contain a DockPanel");
            Assert.That(_dockPanel.Parent, Is.SameAs(_window.Window), "The parent element of the DockPanel is the Window");
        }

        [MonitoredTest("The DockPanel Should contain 4 Buttons And an Image"), Order(2)]
        public void _02_TheDockPanelShouldContain4ButtonsAndAnImage()
        {
            Assert.That(_buttons.Count, Is.EqualTo(4), "The DockPanel should contain 4 buttons");
            Assert.That(_image.Parent, Is.SameAs(_dockPanel), "The parent element of the DockPanel is the Window");
        }

        [MonitoredTest("The Buttons in the DockPanel Should have all margins of 5 and a Width of 80 and a Height of 30"), Order(3)]
        public void _03_TheDockPanelShouldContain4ButtonsAndAnImage()
        {
            Assert.That(_buttons.All(b=>b.Height==30),Is.True, "All buttons should hava a Height of 30");
            Assert.That(_buttons.All(b =>b.Width == 80), Is.True, "All buttons should hava a Width of 80");
            Assert.That(_buttons.All(b => b.Margin.Left==5), Is.True, "All buttons should hava a Left Margin of 5");
            Assert.That(_buttons.All(b => b.Margin.Right == 5), Is.True, "All buttons should hava a Right Margin of 5");
            Assert.That(_buttons.All(b => b.Margin.Top == 5), Is.True, "All buttons should hava a Top Margin of 5");
            Assert.That(_buttons.All(b => b.Margin.Bottom == 5), Is.True, "All buttons should hava a Bottom Margin of 5");
        }

        [MonitoredTest("All Buttons Should have a Click Event Handler"), Order(4)]
        public void _04_AllButtonsShouldHaveAClickEventHandler()
        {
            Assert.That(_buttons.All(b=>VerifyClickEventHandler(b, "Click")), Is.True, "The button in the StackPanel should have a click event handler.");
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
        [MonitoredTest("All Buttons Should have a the correct DockPanel.Dock Value"), Order(5)]
        public void _05_AllButtonsShouldHaveTheCorrectDockPanelDock()
        {
            Assert.That(_topButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Top), "The Top Button should hava a 'Top' DockPanal.Dock Value");
            Assert.That(_leftButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Left), "The Left Button should hava a 'Left' DockPanal.Dock Value");
            Assert.That(_rightButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Right), "The Right Button should hava a 'Right' DockPanal.Dock Value");
            Assert.That(_bottomButton.GetValue(DockPanel.DockProperty), Is.EqualTo(Dock.Bottom), "The Bottom Button should hava a 'Bottom' DockPanal.Dock Value");
        }

        [MonitoredTest("The RenderTransform and RenderTransformOrigin properties of The image should be set"), Order(6)]
        public void _06_TheRenderTransformAndRenderTransformOriginPropertiesOfTheImageShouldBeSet()
        {
            Assert.That(_image.RenderTransform, Is.Not.Null, "The image should have a RenderTransform property");
            RotateTransform rotateTransform = (RotateTransform)_image.RenderTransform;
            Assert.That(rotateTransform.Angle, Is.EqualTo(0), "The angle of the rotatetransform property of the image should be 0");
            Assert.That(rotateTransform.CenterX, Is.EqualTo(0), "The CenterX property of the rotatetransform property of the image should be 0");
            Assert.That(rotateTransform.CenterY, Is.EqualTo(0), "The CenterY property of the rotatetransform property of the image should be 0");

            Assert.That(_image.RenderTransformOrigin, Is.Not.Null, "The RenderTransformOrigin property of het image should have a value");

            Assert.That(_image.RenderTransformOrigin.X, Is.EqualTo(0.5), "The RenderTransformOrigin.X property of het image should have a value of 0.5");
            Assert.That(_image.RenderTransformOrigin.Y, Is.EqualTo(0.5), "The RenderTransformOrigin.Y property of het image should have a value of 0.5");
        }

        
        [MonitoredTest("The image should rotate to the right when clicking the Right Button"), Order(7)]
        public void _07_TheImageShoudRotateToTheRightWhenClickingTheRightButton()
        {
            _rightButton.FireClickEvent();
            DispatcherUtil.DoEvents();
            RotateTransform rotateTransform = (RotateTransform)_image.RenderTransform;
            Assert.That(rotateTransform.Angle, Is.EqualTo(90), "The angle of the rotatetransform property of the image should be 90 after clicking the right button");
        }

        [MonitoredTest("The image should rotate to the bottom when clicking the Bottom Button"), Order(8)]
        public void _08_TheImageShoudRotateToTheBottomWhenClickingTheBottomButton()
        {
            _bottomButton.FireClickEvent();
            DispatcherUtil.DoEvents();
            RotateTransform rotateTransform = (RotateTransform)_image.RenderTransform;
            Assert.That(rotateTransform.Angle, Is.EqualTo(180), "The angle of the rotatetransform property of the image should be 180 after clicking the bottom button");
        }
        [MonitoredTest("The image should rotate to the left when clicking the Left Button"), Order(9)]
        public void _09_TheImageShoudRotateToTheLeftWhenClickingTheLeftButton()
        {
            _leftButton.FireClickEvent();
            DispatcherUtil.DoEvents();
            RotateTransform rotateTransform = (RotateTransform)_image.RenderTransform;
            Assert.That(rotateTransform.Angle, Is.EqualTo(270), "The angle of the rotatetransform property of the image should be 270 after clicking the left button");
        }
        [MonitoredTest("The image should not rotate when clicking the Top Button"), Order(10)]
        public void _10_TheImageShoudNotRotateWhenClickingTheUpButton()
        {
            _topButton.FireClickEvent();
            DispatcherUtil.DoEvents();
            RotateTransform rotateTransform = (RotateTransform)_image.RenderTransform;
            Assert.That(rotateTransform.Angle, Is.EqualTo(0), "The angle of the rotatetransform property of the image should be 0 after clicking the Up button");
        }




    }
}
