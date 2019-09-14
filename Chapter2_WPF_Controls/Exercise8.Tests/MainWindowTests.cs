using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise8.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise08", 
        @"Exercise8\App.xaml;Exercise8\MainWindow.xaml")]
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
            var codeBehindFilePath = @"Exercise8\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("78-BB-A0-13-AB-4A-3C-27-E9-41-7B-98-17-18-BD-A4"), () =>
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Should have a button with a custom template"), Order(2)]
        public void _2_ShouldHaveAButtonWithACustomTemplate()
        {
            AssertHasButtonWithCustomTemplate();
        }

        [MonitoredTest("The template should have a grid with overlapping ellipses and a contentpresenter"), Order(3)]
        public void _3_TheTemplateShouldHaveAGridWithOverlappingEllipsesAndContentPresenter()
        {
            AssertHasButtonWithCustomTemplate();

            var grid = GetAndAssertGrid();
            Assert.That(double.IsNaN(grid.Width), Is.True, () => "The 'Grid' should not have a fixed 'Width'.");
            Assert.That(double.IsNaN(grid.Height), Is.True, () => "The 'Grid' should not have a fixed 'Height'.");

            var ellipses = GetAndAssertEllipses(grid);
            Assert.That(ellipses,
                Has.All.Matches((Ellipse ellipse) =>
                    double.IsNaN(ellipse.Width) &&
                    double.IsNaN(ellipse.Height)),
                () => "None of the ellipses should have a fixed 'Width' or 'Height'.");

            Assert.That(ellipses,
                Has.All.Matches((Ellipse ellipse) => Equals(ellipse.Parent, grid)),
                () => "Both ellipses need to be direct children of the 'Grid'.");

            Assert.That(ellipses,
                Has.All.Matches((Ellipse ellipse) =>
                    ellipse.HorizontalAlignment == HorizontalAlignment.Stretch &&
                    ellipse.VerticalAlignment == VerticalAlignment.Stretch),
                () => "Both ellipses need to stretch themselves to the borders of the grid. " +
                      "This can be achieved by not setting 'HorizontalAlignment' and 'VerticalAlignment' so that the default (Stretch) is used.");

            Assert.That(ellipses,
                Has.One.Matches((Ellipse ellipse) =>
                    ellipse.Margin.Left > 0 &&
                    ellipse.Margin.Top > 0 &&
                    ellipse.Margin.Right > 0 &&
                    ellipse.Margin.Bottom > 0),
                () =>
                    "One of the ellipses (the outer ellipse) should have a 'Margin' of zero on all sides so that it stretches right onto the border of the grid");

            Assert.That(ellipses,
                Has.One.Matches((Ellipse ellipse) =>
                    ellipse.Margin.Left > 0 &&
                    ellipse.Margin.Top > 0 &&
                    ellipse.Margin.Right > 0 &&
                    ellipse.Margin.Bottom > 0),
                () =>
                    "One of the ellipses (the inner ellipse) should have a 'Margin' on all sides to make it smaller than the outer ellipse");

            Assert.That(ellipses,
                Has.All.Matches((Ellipse ellipse) =>
                    ellipse.Fill is LinearGradientBrush),
                () => "Both ellipses need to have their 'Fill' property set to a 'LinearGradientBrush'.");

            Assert.That(ellipses,
                Has.All.Matches((Ellipse ellipse) => Equals(ellipse.RenderTransform, Transform.Identity)),
                () => "The 'RenderTransform' property of all ellipses should be 'Transform.Identity' when the button is not pressed. " +
                      "Only apply a (scale) transformation when the button is pressed.");

            var contentPresenter = grid.FindVisualChildren<ContentPresenter>().FirstOrDefault();
            Assert.That(contentPresenter, Is.Not.Null, () => "The 'Grid' control should contain an instance of 'ContentPresenter'. " +
                                                             "This is the placeholder for the content of the button. " +
                                                             "When the 'Content' of a 'Button' with this template is normal text, WPF will put a 'TextBlock' here.");
            Assert.That(contentPresenter.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center),
                () =>
                    "The 'HorizontalAlignment' of the 'ContentPresenter' should be 'Center' " +
                    "so that content is placed in the middle of the button");
            Assert.That(contentPresenter.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center),
                () =>
                    "The 'VerticalAlignment' of the 'ContentPresenter' should be 'Center' " +
                    "so that content is placed in the middle of the button");
        }

        [MonitoredTest("The border of the button should become black on mouseover"), Order(4)]
        public void _4_TheBorderOfTheButtonShouldBecomeBlackOnMouseOver()
        {
            AssertHasButtonWithCustomTemplate();

            var mouseOverTrigger = _buttonTemplate.Triggers.OfType<Trigger>().FirstOrDefault(trigger => trigger.Property.Name == "IsMouseOver");
            Assert.That(mouseOverTrigger, Is.Not.Null,
                () =>
                    "No 'Trigger' for 'Property' 'IsMouseOver' could be found in the 'Triggers' collection of the template.");
            Assert.That(mouseOverTrigger.Value, Is.True,
                () => "The trigger for 'IsMouseOver' should be activated when the 'Value' is true.");
            var fillSetter = mouseOverTrigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property.Name == "Fill");
            Assert.That(fillSetter, Is.Not.Null,
                () => "The trigger for 'IsMouseOver' should have a 'Setter' for the 'Property' 'Fill'.");
            Assert.That(fillSetter.Value, Is.TypeOf<SolidColorBrush>(),
                () => "The 'Value' for the 'Setter' for the 'Property' 'Fill' should be a 'Black' 'SolidColorBrush'.");

            Assert.That(fillSetter.TargetName, Is.Not.Empty,
                () =>
                    "The 'Setter for the 'Property' 'Fill' should target the outer ellipse. " +
                    "You can do this by setting the 'TargetName' property.");

            var grid = GetAndAssertGrid();
            var ellipses = GetAndAssertEllipses(grid);
            var outerEllipse = ellipses.FirstOrDefault(ellipse => ellipse.Margin.Top == 0.0);
            Assert.That(outerEllipse, Is.Not.Null,
                () =>
                    "The 'Setter for the 'Property' 'Fill' should target the outer ellipse, " +
                    "but no 'Ellipse' with a 'Margin' of zero is found.");

            Assert.That(fillSetter.TargetName, Is.EqualTo(outerEllipse.Name),
                () =>
                    "The 'Setter for the 'Property' 'Fill' should target the outer ellipse, " +
                    "but the 'TargetName' does not match the 'x:Name' of the 'Ellipse'.");
        }

        [MonitoredTest("The button should become smaller when pressed"), Order(5)]
        public void _5_TheButtonShouldBecomeSmallerWhenPressed()
        {
            AssertHasButtonWithCustomTemplate();

            var pressedTrigger = _buttonTemplate.Triggers.OfType<Trigger>().FirstOrDefault(trigger => trigger.Property.Name == "IsPressed");
            Assert.That(pressedTrigger, Is.Not.Null,
                () =>
                    "No 'Trigger' for 'Property' 'IsPressed' could be found in the 'Triggers' collection of the template.");
            Assert.That(pressedTrigger.Value, Is.True,
                () => "The trigger for 'IsPressed' should be activated when the 'Value' is true.");

            //render transform
            var renderTransformSetter = pressedTrigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property.Name == "RenderTransform");
            Assert.That(renderTransformSetter, Is.Not.Null,
                () => "The trigger for 'IsPressed' should have a 'Setter' for the 'Property' 'RenderTransform'. " +
                      "This property makes it possible to apply a transformation on the button (scale, rotate, skew).");
            var scaleTransForm = renderTransformSetter.Value as ScaleTransform;
            Assert.That(scaleTransForm, Is.Not.Null,
                () => "The 'Value' for the 'Setter' for the 'Property' 'RenderTransform' should be an instance of 'ScaleTransform'.");
            Assert.That(scaleTransForm.ScaleX, Is.LessThan(1.0),
                () =>
                    "The 'ScaleX' property of the 'ScaleTransform' should be less than 1 " +
                    "(e.g. 0.8 which horizontally scales the button to 80% of its size).");
            Assert.That(scaleTransForm.ScaleY, Is.LessThan(1.0),
                () =>
                    "The 'ScaleY' property of the 'ScaleTransform' should be less than 1 " +
                    "(e.g. 0.8 which vertically scales the button to 80% of its size).");

            Assert.That(renderTransformSetter.TargetName, Is.Not.Empty,
                () =>
                    "The 'Setter for the 'Property' 'Fill' should target the outer ellipse. " +
                    "You can do this by setting the 'TargetName' property.");

            //render transform origin
            var renderTransformOriginSetter = pressedTrigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property.Name == "RenderTransformOrigin");
            Assert.That(renderTransformOriginSetter, Is.Not.Null,
                () => "The trigger for 'IsPressed' should have a 'Setter' for the 'Property' 'RenderTransformOrigin'. " +
                      "This property makes it possible to tell where the transformation should originate. " +
                      "In this case we want the button to scale from the center.");

            var origin = (Point)renderTransformOriginSetter.Value;
            Assert.That(origin.X == 0.5 && origin.Y == 0.5, Is.True,
                () =>
                    "The 'Value' for the 'Setter' for the 'Property' 'RenderTransformOrigin' " +
                    "should be '0.5,0.5' so that the origin will be at 50% in both directions.");
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

        private static List<Ellipse> GetAndAssertEllipses(Grid grid)
        {
            var ellipses = grid.FindVisualChildren<Ellipse>().ToList();
            Assert.That(ellipses, Has.Count.EqualTo(2), () => "The 'Grid' control should contain 2 instances of 'Ellipse'.");
            return ellipses;
        }

        private Grid GetAndAssertGrid()
        {
            var grid = _buttonTemplate.LoadContent() as Grid;
            Assert.That(grid, Is.Not.Null, () => "The 'Content' of the 'ControlTemplate' should be a grid");
            return grid;
        }

    }
}
