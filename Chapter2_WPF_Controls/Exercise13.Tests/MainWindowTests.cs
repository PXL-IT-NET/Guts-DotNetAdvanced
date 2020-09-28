using System.Collections.Generic;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Exercise13.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise13", @"Exercise13\MainWindow.xaml;Exercise13\App.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Button _arrowButton;
        private Button _pentagonButton;
        private TextBox _textBox;
        private ControlTemplate _roundedCornerTextBoxTemplate;
        private ControlTemplate _arrowButtonTemplate;
        private ControlTemplate _pentagonButtonTemplate;
        private App _app;

        [OneTimeSetUp]
        public void Setup()
        {
            _app = new App();
            _app.InitializeComponent(); //parses the app.xaml and loads the resources

            _window = new TestWindow<MainWindow>();

            _textBox = _window.GetUIElements<TextBox>().FirstOrDefault();

            var allButtons = _window.GetUIElements<Button>().ToList();
            if (allButtons.Count >= 1)
            {
                _arrowButton = allButtons.ElementAt(0);
                _pentagonButton = allButtons.ElementAt(1);
            }

            if (_textBox != null)
            {
                _roundedCornerTextBoxTemplate = _textBox.Template;
            }
            if (_arrowButton != null)
            {
                _arrowButtonTemplate = _arrowButton.Template;
            }
            if (_pentagonButton != null)
            {
                _pentagonButtonTemplate = _pentagonButton.Template;
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
            var codeBehindFilePath = @"Exercise13\MainWindow.xaml.cs";

            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("D3-78-DF-1E-D7-B3-8E-C3-51-B7-A7-AD-13-ED-34-5B"),
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Should have a TextBox and 2 buttons"), Order(2)]
        public void _2_ShouldHaveOneTextBoxAnd2Buttons()
        {
            AssertHasOneTextBoxAndTwoButtons();
        }

        [MonitoredTest("Should have a TextBox with a custom template"), Order(3)]
        public void _3_ShouldHaveOneTextBoxWithACustomTemplate()
        {
            AssertHasOneTextBoxWithCustomTemplate();
            AssertIsTopLeftAlignedWithFixedDimensions(_textBox);
        }

        [MonitoredTest("Should have two Buttons with a custom template"), Order(4)]
        public void _4_ShouldHaveTwoButtonsWithACustomTemplate()
        {
            AssertHasTwoButtonsWithCustomTemplate();
            AssertIsTopLeftAlignedWithFixedDimensions(_arrowButton);
            AssertIsTopLeftAlignedWithFixedDimensions(_pentagonButton);
        }

        [MonitoredTest("The TextBox Should have a template with a Border and a ScrollViewer"), Order(5)]
        public void _5_TheTextBoxTemplateShouldHaveABorderWithAndScrollViewer()
        {
            AssertHasOneTextBoxWithCustomTemplate();

            Border border = GetAndAssertBorder();

            SolidColorBrush solidBorderBrush = border.BorderBrush as SolidColorBrush;
            Assert.That(solidBorderBrush, Is.Not.Null, "The 'BorderBrush' of the 'Border' should be a solid color.");

            Assert.That(border.BorderThickness, Is.Not.Null, "The 'BorderThickness' of the 'Border' should be set.");
            Assert.That(border.BorderThickness.Bottom, Is.GreaterThan(0), "The thickness of the border should be positive.");
            Assert.That(border.BorderThickness.Bottom, Is.EqualTo(border.BorderThickness.Left), "The thickness should be equal on all sides.");
            Assert.That(border.BorderThickness.Left, Is.EqualTo(border.BorderThickness.Right), "The thickness should be equal on all sides.");
            Assert.That(border.BorderThickness.Right, Is.EqualTo(border.BorderThickness.Top), "The thickness should be equal on all sides.");

            Assert.That(border.CornerRadius.TopLeft, Is.GreaterThanOrEqualTo(0), "The TopLeft corner radius of the 'Border' should be positive.");
            Assert.That(border.CornerRadius.TopRight, Is.EqualTo(0), "The TopRight corner radius of the 'Border' should be 0.");
            Assert.That(border.CornerRadius.BottomRight, Is.GreaterThanOrEqualTo(0), "The BottomRight corner radius of the 'Border' should be positive.");
            Assert.That(border.CornerRadius.BottomLeft, Is.EqualTo(0), "The BottomLeft corner radius or the 'Border' should be 0.");

            Assert.That(border.Child, Is.TypeOf<ScrollViewer>(), "The 'Border' does not have a 'ScrollViewer' child element.");

            Assert.That(((ScrollViewer)border.Child).Name, Is.EqualTo("PART_ContentHost"),
                "The Name of the 'ScrollViewer' has to be 'PART_ContentHost'. " +
                "See: https://docs.microsoft.com/dotnet/desktop/wpf/controls/textbox-styles-and-templates .");
        }

        [MonitoredTest("The arrow Button should have a template with a Polygon and a ContentPresenter"), Order(6)]
        public void _6_TheArrowButtonTemplateShouldHaveAPolygonAndContentPresenter()
        {
            Grid grid = GetAndAssertGridOfPolygonTemplate();

            Assert.That(grid, Is.Not.Null, "The Template for the Polygon Button should contain a Grid.");

            Polygon polygon = grid.Children.OfType<Polygon>().FirstOrDefault();
            Assert.That(polygon, Is.Not.Null, "The grid in the polygon template should contain a 'Polygon'.");
            Assert.That(polygon.Points.Count, Is.EqualTo(6), "The polygon on the button should contain 6 points");
            Assert.That(polygon.Stretch, Is.EqualTo(Stretch.Fill),
                "The polygon should stretch to fill the whole control (no matter what the size of the control is");

            SolidColorBrush solidStrokeBrush = polygon.Stroke as SolidColorBrush;
            Assert.That(solidStrokeBrush, Is.Not.Null, "The 'Stroke' of the 'Polygon' should be a solid color.");

            SolidColorBrush solidFillBrush = polygon.Fill as SolidColorBrush;
            Assert.That(solidFillBrush, Is.Not.Null, "The 'Fill' of the 'Polygon' should be a solid color.");

            ContentPresenter contentPresenter = grid.Children.OfType<ContentPresenter>().FirstOrDefault();
            Assert.That(contentPresenter, Is.Not.Null,
                "The 'grid' control should contain an instance of 'ContentPresenter'. " +
                "This is the placeholder for the content of the button. " +
                "When the 'Content' of a 'Button' with this template is normal text, WPF will put a 'TextBlock' here.");

            Assert.That(contentPresenter.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center),
                "The 'HorizontalAlignment' of the 'ContentPresenter' should be 'Center' " +
                "so that content is placed in the middle of the button");

            Assert.That(contentPresenter.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center),
                "The 'VerticalAlignment' of the 'ContentPresenter' should be 'Center' " +
                "so that content is placed in the middle of the button");
        }

        [MonitoredTest("The Pentagon Button should have a template with a polygon and a ContentPresenter"), Order(7)]
        public void _7_ThePentagonButtonTemplateShouldHaveAPolygonAndContentPresenter()
        {
            Grid grid = GetAndAssertGridOfPentagonTemplate();

            Assert.That(grid, Is.Not.Null, "The Template for the Pentagon Button should contain a Grid.");

            Polygon polygon = grid.Children.OfType<Polygon>().FirstOrDefault();
            Assert.That(polygon, Is.Not.Null, "The grid in the pentagon template should contain a 'Polygon'.");
            Assert.That(polygon.Points.Count, Is.EqualTo(5), "The polygon on the button should contain 5 points");
            Assert.That(polygon.Stretch, Is.EqualTo(Stretch.Fill),
                "The polygon should stretch to fill the whole control (no matter what the size of the control is");

            SolidColorBrush solidStrokeBrush = polygon.Stroke as SolidColorBrush;
            Assert.That(solidStrokeBrush, Is.Not.Null, "The 'Stroke' of the 'Polygon' should be a solid color.");

            SolidColorBrush solidFillBrush = polygon.Fill as SolidColorBrush;
            Assert.That(solidFillBrush, Is.Not.Null, "The 'Fill' of the 'Polygon' should be a solid color.");

            ContentPresenter contentPresenter = grid.Children.OfType<ContentPresenter>().FirstOrDefault();
            Assert.That(contentPresenter, Is.Not.Null,
                "The 'grid' control should contain an instance of 'ContentPresenter'. " +
                "This is the placeholder for the content of the button. " +
                "When the 'Content' of a 'Button' with this template is normal text, WPF will put a 'TextBlock' here.");

            Assert.That(contentPresenter.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center),
                "The 'HorizontalAlignment' of the 'ContentPresenter' should be 'Center' " +
                "so that content is placed in the middle of the button");
            Assert.That(contentPresenter.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center),
                "The 'VerticalAlignment' of the 'ContentPresenter' should be 'Center' " +
                "so that content is placed in the middle of the button");

        }

        private Grid GetAndAssertGridOfPolygonTemplate()
        {
            Assert.That(_arrowButtonTemplate, Is.Not.Null, "No template found for the arrow button.");
            var grid = _arrowButtonTemplate.LoadContent() as Grid;
            Assert.That(grid, Is.Not.Null, "The 'Content' of the 'ControlTemplate' of the arrow Button should be a grid");
            return grid;
        }

        private Grid GetAndAssertGridOfPentagonTemplate()
        {
            Assert.That(_pentagonButtonTemplate, Is.Not.Null, "No template found for the pentagon button.");
            var grid = _pentagonButtonTemplate.LoadContent() as Grid;
            Assert.That(grid, Is.Not.Null, () => "The 'Content' of the 'ControlTemplate' of the pentagon Button should be a grid");
            return grid;
        }

        private void AssertIsTopLeftAlignedWithFixedDimensions(Control control)
        {
            Assert.That(control.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Left),
                $"The 'HorizontalAlignment' of the {control.GetType().Name} (the control itself, not the template) should be 'Left'.");
            Assert.That(control.VerticalAlignment, Is.EqualTo(VerticalAlignment.Top),
                $"The 'VerticalAlignment' of the {control.GetType().Name} (the control itself, not the template) should be 'Top'.");
            Assert.That(control.Width, Is.GreaterThan(0),
                $"The 'Width' of the {control.GetType().Name} (the control itself, not the template) should be set.");
            Assert.That(control.Height, Is.GreaterThan(0),
                $"The 'Height' of the {control.GetType().Name} (the control itself, not the template) should be set.");
        }

        private Border GetAndAssertBorder()
        {
            var border = _roundedCornerTextBoxTemplate.LoadContent() as Border;
            Assert.That(border, Is.Not.Null, "The 'Content' of the 'ControlTemplate' of the TextBox should be a border");
            return border;
        }

        private void AssertHasTwoButtonsWithCustomTemplate()
        {
            Assert.That(_arrowButton, Is.Not.Null, "No 'Button' control could be found for the arrow.");
            Assert.That(_pentagonButton, Is.Not.Null, "No 'Button' control could be found for the pentagon.");

            List<ControlTemplate> customTemplatesForButtons = _app.Resources.Values.OfType<ControlTemplate>()
                .Where(c => c.TargetType.Name == "Button").ToList();

            Assert.That(customTemplatesForButtons.Count, Is.EqualTo(2),
                "There should be exactly 2 Button 'ControlTemplates' in the resources of the application (App.xaml).");

            Assert.That(customTemplatesForButtons.Any(t => t == _arrowButtonTemplate), Is.True,
                "The 'PolygonButton' should have its 'Template' property set to the 'ControlTemplate' defined in the application resources.");

            Assert.That(customTemplatesForButtons.Any(t => t == _pentagonButtonTemplate), Is.True,
                "The 'PentagonButton' should have its 'Template' property set to the 'ControlTemplate' defined in the application resources.");
        }

        private void AssertHasOneTextBoxWithCustomTemplate()
        {
            Assert.That(_textBox, Is.Not.Null, "No 'TextBox' control could be found.");

            var customTemplateForTextBox = _app.Resources.Values.OfType<ControlTemplate>().FirstOrDefault(c => c.TargetType.Name == "TextBox");

            Assert.That(customTemplateForTextBox, Is.Not.Null, "No 'ControlTemplates' found in the resources of the application (App.xaml) for the TextBox");
            Assert.That(_roundedCornerTextBoxTemplate, Is.EqualTo(customTemplateForTextBox),
                "The 'TextBox' should have its 'Template' property set to the 'ControlTemplate' defined in the application resources.");
        }

        private void AssertHasOneTextBoxAndTwoButtons()
        {
            Assert.That(_textBox, Is.Not.Null, "The textBox could not be found.");
            Assert.That(_arrowButton, Is.Not.Null, "The Arrow button could not be found.");
            Assert.That(_pentagonButton, Is.Not.Null, "The Pentagon button could not be found.");
        }
    }
}