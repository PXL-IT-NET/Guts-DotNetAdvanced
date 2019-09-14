using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Classic.UI;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise11.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise11", 
        @"Exercise11\App.xaml;Exercise11\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Button _roundButton;
        private ControlTemplate _buttonTemplate;
        private App _app;

        [OneTimeSetUp]
        public void Setup()
        {
            _app = new App();
            _app.InitializeComponent(); //parses the app.xaml and loads the resources

            _window = new TestWindow<MainWindow>();
            _roundButton = _window.GetUIElements<Button>().FirstOrDefault();
            if (_roundButton != null)
            {
                _buttonTemplate = _roundButton.Template;
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _app.Shutdown();
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _1_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise11\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("CF-85-58-E7-D0-59-4B-AB-1D-6B-CC-D1-F9-89-E4-D3"), () =>
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Should have a button with a custom template"), Order(2)]
        public void _2_ShouldHaveAButtonWithACustomTemplate()
        {
            AssertHasButtonWithCustomTemplate();
        }

        [MonitoredTest("The template should have a grid with a triangle polygon and a contentpresenter"), Order(3)]
        public void _3_TheTemplateShouldHaveAGridWithATrianglePolygonAndContentPresenter()
        {
            AssertHasButtonWithCustomTemplate();

            var grid = GetAndAssertGrid();
            Assert.That(double.IsNaN(grid.Width), Is.True, () => "The 'Grid' should not have a fixed 'Width'.");
            Assert.That(double.IsNaN(grid.Height), Is.True, () => "The 'Grid' should not have a fixed 'Height'.");

            var triangle = GetAndAssertTriangle(grid);
            Assert.That(triangle.Fill.ToString(), Contains.Substring("D3D3D3").IgnoreCase, "The 'Fill' of the 'Polygon' should be 'LightGray'.");
            Assert.That(triangle.Stretch, Is.EqualTo(Stretch.Fill), "The 'Polygon' should 'Stretch' to 'Fill' the available space.");

            var points = triangle.Points;
            Assert.That(points, Has.Count.EqualTo(3), "The 'Polygon' should contain 3 'Points'.");
            var orderdPoints = points.OrderBy(p => p.X).ToList();
            Assert.That(orderdPoints[0].X, Is.EqualTo(0), 
                "The bottom left point of the triangle should have an X value of 0.");
            Assert.That(orderdPoints[0].Y, Is.GreaterThan(0), 
                "The bottom left point of the triangle should have an Y value greather than 0.");
            Assert.That(orderdPoints[0].Y, Is.EqualTo(orderdPoints[2].Y), 
                "The bottom left point of the triangle should have the same Y value as the bottom right point.");
            Assert.That(orderdPoints[1].X, Is.EqualTo((orderdPoints[2].X - orderdPoints[0].X)/ 2.0),
                "The top middle point of the triangle should have an X value that is in the middle.");
            Assert.That(orderdPoints[1].Y, Is.EqualTo(0),
                "The top middle point of the triangle should have an Y value of 0.");

            Assert.That(triangle.Parent,Is.SameAs(grid),
                () => "The triangle must be a direct child of the 'Grid'.");

            var contentPresenter = grid.FindVisualChildren<ContentPresenter>().FirstOrDefault();
            Assert.That(contentPresenter, Is.Not.Null, () => "The 'Grid' control should contain an instance of 'ContentPresenter'. " +
                                                             "This is the placeholder for the content of the button. " +
                                                             "When the 'Content' of a 'Button' with this template is normal text, WPF will put a 'TextBlock' here.");
            Assert.That(contentPresenter.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center),
                () =>
                    "The 'HorizontalAlignment' of the 'ContentPresenter' should be 'Center' " +
                    "so that content is placed in the middle of the button");
            Assert.That(contentPresenter.VerticalAlignment, Is.EqualTo(VerticalAlignment.Bottom),
                () =>
                    "The 'VerticalAlignment' of the 'ContentPresenter' should be 'Bottom' " +
                    "so that content is placed in the middle of the button");

            Assert.That(contentPresenter.Margin.Bottom, Is.GreaterThan(0), "The 'ContentPresenter' must have some 'Margin' at the bottom.");
        }

        [MonitoredTest("The button should rotate once when clicked"), Order(4)]
        public void _4_TheButtonShouldRotateWhenClicked()
        {
            AssertHasButtonWithCustomTemplate();

            var grid = GetAndAssertGrid();
            var rotateTransform = grid.RenderTransform as RotateTransform;
            Assert.That(rotateTransform, Is.Not.Null, "The 'RenderTransform' of the 'Grid' should be set to an instance of 'RotateTransform'.");

            Assert.That(grid.RenderTransformOrigin.X, Is.EqualTo(0.5), "The 'RenderTransformOrigin' of the 'Grid' should be in the center (0.5,0.5).");
            Assert.That(grid.RenderTransformOrigin.Y, Is.EqualTo(0.5), "The 'RenderTransformOrigin' of the 'Grid' should be in the center (0.5,0.5).");

            var clickEventTrigger = _buttonTemplate.Triggers.OfType<EventTrigger>().FirstOrDefault(trigger => trigger.RoutedEvent.Name == "Click");
            Assert.That(clickEventTrigger, Is.Not.Null,
                () =>
                    "No 'EventTrigger' with 'RoutedEvent' 'Button.Click' could be found in the 'Triggers' collection of the template.");

            var beginStoryBoard = clickEventTrigger.Actions.OfType<BeginStoryboard>().FirstOrDefault();
            Assert.That(beginStoryBoard, Is.Not.Null, "The 'EventTrigger' should contain a 'BeginStoryboard' instance.");

            var storyboard = beginStoryBoard.Storyboard;
            Assert.That(storyboard, Is.Not.Null,
                "The 'BeginStoryboard' instance should contain a 'Storyboard' instance.");

            var doubleAnimation = storyboard.Children.OfType<DoubleAnimation>().FirstOrDefault();
            Assert.That(doubleAnimation, Is.Not.Null, "the 'Storyboard' instance should contain a 'DoubleAnimation' instance.");

            var targetName = Storyboard.GetTargetName(doubleAnimation);
            Assert.That(targetName, Is.Not.Empty, "The 'StoryBoard.TargetName' attached property should be set for the 'DoubleAnimation'.");

            rotateTransform = _buttonTemplate.FindName(targetName, _roundButton) as RotateTransform;
            Assert.That(rotateTransform, Is.Not.Null, 
                $"Cannot find a 'RotateTransform' with the name {targetName}. " +
                "You should name the 'RotateTransfrom' so that it can be animated.");

            var angleProperty = Storyboard.GetTargetProperty(doubleAnimation);
            Assert.That(angleProperty.Path, Is.EqualTo("Angle"), "The 'StoryBoard.TargetProperty' attached property should be set to 'Angle' for the 'DoubleAnimation'.");

            Assert.That(doubleAnimation.From, Is.EqualTo(0.0), "The animation should start 'From' 0.0 degrees.");
            Assert.That(doubleAnimation.From, Is.EqualTo(0.0), "The animation should go 'To' 360.0 degrees.");
            Assert.That(doubleAnimation.Duration.TimeSpan.TotalMilliseconds, Is.EqualTo(250), "The 'Duration' of the animation should be 250 milliseconds.");
            Assert.That(doubleAnimation.RepeatBehavior.Count, Is.EqualTo(1), "The animation should not repeat.");
        }

        private void AssertHasButtonWithCustomTemplate()
        {
            Assert.That(_roundButton, Is.Not.Null, () => "No 'Button' control could be found.");

            var customTemplate = _app.Resources.Values.OfType<ControlTemplate>().FirstOrDefault();
            Assert.That(customTemplate, Is.Not.Null,
                () => "No 'ControlTemplate' found in the resources of the application (App.xaml).");
            Assert.That(customTemplate.TargetType.Name, Contains.Substring("Button"),
                () => "The 'ControlTemplate' should target 'Button' (or 'ButtonBase').");

            Assert.That(_buttonTemplate, Is.EqualTo(customTemplate),
                () =>
                    "The 'Button' should have its 'Template' property set to the 'ControlTemplate' defined in the application resources.");
        }

        private static Polygon GetAndAssertTriangle(Grid grid)
        {
            var triangle = grid.FindVisualChildren<Polygon>().FirstOrDefault();
            Assert.That(triangle, Is.Not.Null, "The 'Grid' control should contain an instance of 'Polygon'.");
            return triangle;
        }

        private Grid GetAndAssertGrid()
        {
            var grid = _buttonTemplate.LoadContent() as Grid;
            Assert.That(grid, Is.Not.Null, () => "The 'Content' of the 'ControlTemplate' should be a grid");
            return grid;
        }

    }
}
