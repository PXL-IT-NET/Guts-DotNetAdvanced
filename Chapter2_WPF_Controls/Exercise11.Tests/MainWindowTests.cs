using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
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
        private Button _playButton;
        private ControlTemplate _buttonTemplate;
        private App _app;

        [OneTimeSetUp]
        public void Setup()
        {
            _app = new App();
            _app.InitializeComponent(); //parses the app.xaml and loads the resources

            _window = new TestWindow<MainWindow>();
            _playButton = _window.GetUIElements<Button>().FirstOrDefault();
            if (_playButton != null)
            {
                _buttonTemplate = _playButton.Template;
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _app.Shutdown();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
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

            Grid grid = GetAndAssertGrid();

            Polygon triangle = GetAndAssertTriangle(grid);
            Assert.That(triangle.Fill.ToString(), Contains.Substring("008000").IgnoreCase, "The 'Fill' of the 'Polygon' should be 'Green'.");
            Assert.That(triangle.Stretch, Is.EqualTo(Stretch.Fill), "The 'Polygon' should 'Stretch' to 'Fill' the available space.");
            Assert.That(triangle.Margin.Left > 0
                        && triangle.Margin.Top > 0
                        && triangle.Margin.Right > 0
                        && triangle.Margin.Bottom > 0, Is.True, "The 'Polygon' should have a margin on all sides so that is does not touch the edges of the 'Grid'.");

            var points = triangle.Points;
            Assert.That(points, Has.Count.EqualTo(3), "The 'Polygon' should contain 3 'Points'.");
            var orderedPoints = points.OrderBy(p => p.Y).ToList();
            Assert.That(orderedPoints[0].X, Is.EqualTo(0),
                "The top left point of the triangle should have an X value of 0.");
            Assert.That(orderedPoints[0].Y, Is.EqualTo(0),
                "The top left point of the triangle should have an Y value of 0.");
            Assert.That(orderedPoints[1].X, Is.EqualTo(orderedPoints[2].Y),
                "The right point of the triangle should as far right (X) as the distance from the top to the bottom left point (Y)");
            Assert.That(orderedPoints[1].Y, Is.EqualTo(orderedPoints[2].Y / 2.0),
                "The right point of the triangle should have an Y value that is in the middle.");
            Assert.That(orderedPoints[2].X, Is.EqualTo(0),
                "The bottom left point of the triangle should have an X value of 0.");

            Assert.That(triangle.Parent, Is.SameAs(grid),
                () => "The triangle must be a direct child of the 'Grid'.");

            var contentPresenter = grid.FindVisualChildren<ContentPresenter>().FirstOrDefault();
            Assert.That(contentPresenter, Is.Not.Null,
                "The 'Grid' control should contain an instance of 'ContentPresenter'. " +
                "This is the placeholder for the content of the button. " +
                "When the 'Content' of a 'Button' with this template is normal text, WPF will put a 'TextBlock' here.");
            Assert.That(contentPresenter.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Left),
                "The 'HorizontalAlignment' of the 'ContentPresenter' should be 'Left' " +
                "so that content is placed in the left part of the button");
            Assert.That(contentPresenter.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center),
                "The 'VerticalAlignment' of the 'ContentPresenter' should be 'Center' " +
                "so that content is placed in the middle part of the button");

            Assert.That(contentPresenter.Margin.Left, Is.GreaterThan(0), "The 'ContentPresenter' must have some 'Margin' at the left.");
        }

        [MonitoredTest("The button should have a big white font set"), Order(4)]
        public void _4_TheButtonShouldHaveABigWhiteFontSet()
        {
            AssertHasButtonWithCustomTemplate();

            Assert.That(_playButton.FontWeight, Is.EqualTo(FontWeights.Bold), "The 'FontWeight' of the 'Button' should be 'Bold'.");
            var foregroundBrush = _playButton.Foreground as SolidColorBrush;
            Assert.That(foregroundBrush, Is.Not.Null, "The 'Foreground' of the 'Button' must be a solid color.");
            Assert.That(foregroundBrush.Color.ToString(), Contains.Substring("FFFFFF").IgnoreCase, "The 'Foreground' of the 'Button' should be 'White'.");
            Assert.That(_playButton.FontSize, Is.GreaterThanOrEqualTo(20), "The 'FontSize' of the 'Button' must be at least 20.");
        }

        private void AssertHasButtonWithCustomTemplate()
        {
            Assert.That(_playButton, Is.Not.Null, () => "No 'Button' control could be found.");

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

        private Border GetAndAssertBorder()
        {
            var border = _buttonTemplate.LoadContent() as Border;
            Assert.That(border, Is.Not.Null, "The 'Content' of the 'ControlTemplate' should be a 'Border'.");
            Assert.That(border.Background, Is.Null, "The 'Border' should not have a background.");
            Assert.That(border.BorderBrush, Is.TypeOf<SolidColorBrush>(), "The 'BorderBrush' should be a solid color.");
            Assert.That(border.BorderThickness.Right > 0
                        && border.BorderThickness.Bottom > 0
                        && border.BorderThickness.Left > 0
                        && border.BorderThickness.Top > 0, Is.True, "The 'Border' should have a thickness on all sides.");
            Assert.That(border.CornerRadius.TopLeft > 0
                        && border.CornerRadius.TopRight > 0
                        && border.CornerRadius.BottomRight > 0
                        && border.CornerRadius.BottomLeft > 0, Is.True, "The 'Border' should have a corner radius on all sides.");
            return border;
        }

        private Grid GetAndAssertGrid()
        {
            var border = GetAndAssertBorder();
            var grid = border.Child as Grid;
            Assert.That(grid, Is.Not.Null, "The 'Child' of the 'Border' should be a 'Grid'.");
            Assert.That(double.IsNaN(grid.Width), Is.True, () => "The 'Grid' should not have a fixed 'Width'.");
            Assert.That(double.IsNaN(grid.Height), Is.True, () => "The 'Grid' should not have a fixed 'Height'.");
            return grid;
        }
    }
}
