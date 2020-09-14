using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Exercise13.Tests
{
    [TestFixture]
    //    [ExerciseTestFixture("dotNet2", "H02", "Exercise13", @"Exercise13\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Button _polygonButton;
        private Button _pentagonButton;
        private TextBox _textBox;
        private ControlTemplate _roundedCornerTextBoxTemplate;
        private ControlTemplate _polygonButtonTemplate;
        private ControlTemplate _pentagonButtonTemplate;
        private App _app;

        [OneTimeSetUp]
        public void Setup()
        {
            _app = new App();
            _app.InitializeComponent(); //parses the app.xaml and loads the resources

            _window = new TestWindow<MainWindow>();

            _textBox = (TextBox)_window.GetUIElements<TextBox>().FirstOrDefault();

            var allButtons = _window.GetUIElements<Button>().ToList();
            if (allButtons.Count >= 1)
            {
                _polygonButton = allButtons.ElementAt(0);
                _pentagonButton = allButtons.ElementAt(1);
            }

            if (_textBox != null)
            {
                _roundedCornerTextBoxTemplate = _textBox.Template;
            }
            if (_polygonButton != null)
            {
                _polygonButtonTemplate = _polygonButton.Template;
            }
            if (_pentagonButton != null)
            {
                _pentagonButtonTemplate = _pentagonButton.Template;
            }

            var allTemplates = _app.Resources.Values.OfType<ControlTemplate>().ToList();
            _roundedCornerTextBoxTemplate = allTemplates.Where(t => t.TargetType.Name == "TextBox").FirstOrDefault();
            //         _polygonButtonTemplate = allTemplates[1];
            //         _pentagonButtonTemplate = allTemplates[2];
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
            Assert.That(fileHash, Is.EqualTo("D0-0C-98-BD-0F-FC-F5-EB-F1-8E-27-71-3F-D7-33-6D"), () =>
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Should have a TextBox and 2 buttons"), Order(2)]
        public void _2_ShouldHaveOneTextBoxAnd2Buttons()
        {
            AssertHasOneTextBoxAndTwoButtons();
        }

        private void AssertHasOneTextBoxAndTwoButtons()
        {
            Assert.That(_textBox, Is.Not.Null, () => "The textBox could not be found.");
            Assert.That(_polygonButton, Is.Not.Null, () => "The Polygon button could not be found.");
            Assert.That(_pentagonButton, Is.Not.Null, () => "The Pentagon button could not be found.");
        }
        private void AssertHasTemplates()
        {
            Assert.That(_roundedCornerTextBoxTemplate, Is.Not.Null,
                () => "The 'Resources' collection of the application should contain an instance of 'Style' for the button.");
            Assert.That(_roundedCornerTextBoxTemplate.TargetType.Name, Is.EqualTo("TextBox"),
                () => "A 'Style' instance was found but it does not target a textBox ('TargetType').");
            Assert.That(_polygonButtonTemplate, Is.Not.Null,
                () => "The 'Resources' collection of the application should contain an instance of 'ContentTemplate' for the polygon Button.");
            Assert.That(_polygonButtonTemplate, Is.Not.Null,
                () => "The 'Resources' collection of the application should contain an instance of 'ContentTemplate' for the pentagon Button.");
        }

        [MonitoredTest("Should have a TextBox with a custom template"), Order(3)]

        public void _3_ShouldHaveOneTextBoxWithACustomTemplate()
        {
            AssertHasOneTextBoxWithCustomTemplate();
        }

        private void AssertHasOneTextBoxWithCustomTemplate()
        {
            Assert.That(_textBox, Is.Not.Null, () => "No 'TextBox' control could be found.");

            var customTemplateForTextBox = _app.Resources.Values.OfType<ControlTemplate>().Where(c => c.TargetType.Name == "TextBox").FirstOrDefault();

            Assert.That(customTemplateForTextBox, Is.Not.Null, () => "No 'ControlTemplates' found in the resources of the application (App.xaml) for the TextBox");
            Assert.That(_roundedCornerTextBoxTemplate, Is.EqualTo(customTemplateForTextBox),
                () => "The 'TextBox' should have its 'Template' property set to the 'ControlTemplate' defined in the application resources.");

        }

        [MonitoredTest("Should have two Buttons with a custom template"), Order(4)]
        public void _4_ShouldHaveTwoButtonsWithACustomTemplate()
        {
            AssertHasTwoButtonsWithCustomTemplate();
        }


        private void AssertHasTwoButtonsWithCustomTemplate()
        {
            Assert.That(_polygonButton, Is.Not.Null, () => "No 'Button' control could be found for the polygon.");
            Assert.That(_pentagonButton, Is.Not.Null, () => "No 'Button' control could be found for the pentagon.");

            var customTemplatesForButtons = _app.Resources.Values.OfType<ControlTemplate>().Where(c => c.TargetType.Name == "Button").ToList();

            Assert.That(customTemplatesForButtons.Count, Is.EqualTo(2), () => "No 'ControlTemplates' found in the resources of the application (App.xaml) for both the buttons.");

            Assert.That(_polygonButtonTemplate, Is.EqualTo(customTemplatesForButtons[1]),
                () => "The 'PolygonButton' should have its 'Template' property set to the 'ControlTemplate' defined in the application resources.");

            Assert.That(_pentagonButtonTemplate, Is.EqualTo(customTemplatesForButtons[0]),
                () => "The 'PentagonButton' should have its 'Template' property set to the 'ControlTemplate' defined in the application resources.");

        }

        [MonitoredTest("The TextBox Should have a template with a Border and a ScrollViewer"), Order(5)]
        public void _5_TheTextBoxTemplateShouldHaveABorderWithAndScrollViewer()
        {
            AssertHasOneTextBoxWithCustomTemplate();

            var border = GetAndAssertBorder();
            Assert.That(border.CornerRadius.TopLeft, Is.EqualTo(25), () => "The TopLeft corner or the 'Border' should be 0.");
            Assert.That(border.CornerRadius.TopRight, Is.EqualTo(0), () => "The TopRight corner or the 'Border' should be 25.");
            Assert.That(border.CornerRadius.BottomRight, Is.EqualTo(25), () => "The BottomRight corner or the 'Border' should be 0.");
            Assert.That(border.CornerRadius.BottomLeft, Is.EqualTo(0), () => "The BottomLeft corner or the 'Border' should be 25.");

            Assert.That(border.Child, Is.TypeOf<ScrollViewer>(), () => "The 'Border' doen not have a 'ScrollViewer' child element.");

            Assert.That(((ScrollViewer)border.Child).Name, Is.EqualTo("PART_ContentHost"), () => "The Name of the 'ScrollViewer' has to be 'PART_ContentHost'.");
            Assert.That(((ScrollViewer)border.Child).Focusable, Is.True, () => "The 'ScrollViewer' has to be focussable.");
        }

        private Border GetAndAssertBorder()
        {
            var border = _roundedCornerTextBoxTemplate.LoadContent() as Border;
            Assert.That(border, Is.Not.Null, () => "The 'Content' of the 'ControlTemplate' of the TextBox should be a border");
            return border;
        }

        [MonitoredTest("The Polygon Button should have a template with a Polygon and a ContentPresenter"), Order(6)]
        public void _6_ThePolygonButtonTemplateShouldHaveAPolygonAndContentPresenter()
        {
            var grid = GetAndAssertGridOfPolygonTemplate();

            Assert.That(grid, Is.Not.Null, () => "The Template for the Polygon Button should contain a Grid.");

            var polygon = grid.Children[0] as Polygon;
            Assert.That(polygon.Points.Count, Is.EqualTo(6), () => "The polygon on the button should contain 6 points");

            var contentPresenter = grid.FindVisualChildren<ContentPresenter>().FirstOrDefault();
            Assert.That(contentPresenter, Is.Not.Null, () => "The 'grid' control should contain an instance of 'ContentPresenter'. " +
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

        [MonitoredTest("The Pentagon Button should have a template with a polygon and a ContentPresenter"), Order(7)]
        public void _7_ThePentagonButtonTemplateShouldHaveAPolygonAndContentPresenter()
        {

            var grid = GetAndAssertGridOfPentagonTemplate();

            Assert.That(grid, Is.Not.Null, () => "The Template for the Pentagon Button should contain a Grid.");

            var polygon = grid.Children[0] as Polygon;

            Assert.That(polygon.Points.Count, Is.EqualTo(5), () => "The pentagon on the button should contain 6 points");

            var contentPresenter = grid.FindVisualChildren<ContentPresenter>().FirstOrDefault();
            Assert.That(contentPresenter, Is.Not.Null, () => "The 'grid' control should contain an instance of 'ContentPresenter'. " +
                                                             "This is the placeholder for the content of the button. " +
                                                             "When the 'Content' of a 'Button' with this template is normal text, WPF will put a 'TextBlock' here.");
            //Assert.That(contentPresenter.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center),
            //    () =>
            //        "The 'HorizontalAlignment' of the 'ContentPresenter' should be 'Center' " +
            //        "so that content is placed in the middle of the button");
            //Assert.That(contentPresenter.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center),
            //    () =>
            //        "The 'VerticalAlignment' of the 'ContentPresenter' should be 'Center' " +
            //        "so that content is placed in the middle of the button");

        }

        private Grid GetAndAssertGridOfPolygonTemplate()
        {
            var grid = _polygonButtonTemplate.LoadContent() as Grid;
            var polygon = grid.Children[0] as Polygon;
            Assert.That(grid, Is.Not.Null, () => "The 'Content' of the 'ControlTemplate' of the pentagon Button should be a grid");
            return grid;
        }

        private Grid GetAndAssertGridOfPentagonTemplate()
        {
            var grid = _pentagonButtonTemplate.LoadContent() as Grid;
            var polygon = grid.Children[0] as Polygon;
            Assert.That(grid, Is.Not.Null, () => "The 'Content' of the 'ControlTemplate' of the pentagon Button should be a grid");
            return grid;
        }
    }
}
