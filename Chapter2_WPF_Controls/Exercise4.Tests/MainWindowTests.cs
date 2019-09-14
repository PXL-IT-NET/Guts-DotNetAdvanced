using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise4.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise04", @"Exercise4\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private ToggleButton _toggleButton;
        private GroupBox _ageGroupBox;
        private GroupBox _genderGroupBox;
        private Canvas _canvas;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            _canvas = _window.GetUIElements<Canvas>().FirstOrDefault();
            _toggleButton = _window.GetUIElements<ToggleButton>().FirstOrDefault();
            var groupBoxes = _window.GetUIElements<GroupBox>().ToList();
            _ageGroupBox = groupBoxes.FirstOrDefault(box => (box.Header as string)?.ToLower() == "leeftijd");
            _genderGroupBox = groupBoxes.FirstOrDefault(box => (box.Header as string)?.ToLower() == "geslacht");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _1_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise4\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("0B-07-EE-97-8C-90-C0-82-8D-CF-E1-10-81-B4-E6-84"), () => $"The file '{codeBehindFilePath}' has changed. " +
                                                                             "Undo your changes on the file to make this test pass. " +
                                                                             "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("Should have a (toggle)button"), Order(2)]
        public void _2_ShouldHaveAToggleButton()
        {
            AssertHasToggleButton();
        }

        [MonitoredTest("The (toggle)button should show its (checked) state via its 'Content' property"), Order(3)]
        public void _3_TheToggleButtonShouldShowItsCheckedStateViaItsContentProperty()
        {
            AssertHasToggleButton();

            var style = _toggleButton.Style;
            Assert.That(style, Is.Not.Null, () => "The button has no 'Style' set. The 'Content' property should be changed using a style 'Trigger'.");
            Assert.That(style.TargetType.Name, Is.EqualTo("ToggleButton"),
                () => "A 'Style' instance was found but it does not target (toggle)buttons ('TargetType').");

            var contentSetter = style.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property.Name.ToLower() == "content");
            Assert.That(contentSetter, Is.Not.Null,
                () =>
                    "No 'Setter' found for the 'Content' property. " +
                    "Since the value of the 'Content' property becomes a part of the style of the button, the property should be set in the style.");
            Assert.That(contentSetter.Value, Is.EqualTo("aan").IgnoreCase,
                () => "The 'Value' property of the 'Setter' should be set to 'Aan'.");

            var isCheckedTrigger = style.Triggers.OfType<Trigger>().FirstOrDefault();
            Assert.That(isCheckedTrigger, Is.Not.Null, () => "The style should have a 'Trigger' in its 'Triggers' collection.");
            Assert.That(isCheckedTrigger.Property.Name, Is.EqualTo("IsChecked"), () => "The trigger should be for the 'Property' 'IsChecked'.");
            Assert.That(isCheckedTrigger.Value, Is.False, () => "The trigger should be activated when the 'Value' of 'IsChecked' is 'False'.");

            var triggerContentSetter = isCheckedTrigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property.Name.ToLower() == "content");
            Assert.That(triggerContentSetter, Is.Not.Null,
                () =>
                    "No 'Setter' found in the trigger for the 'Content' property. " +
                    "When the trigger is activated a 'Setter' should set the 'Value' of the 'Content' property to 'Uit'.");
            Assert.That(triggerContentSetter.Value, Is.EqualTo("uit").IgnoreCase,
                () => "The 'Value' property of the 'Setter' in the 'Trigger' should be set to 'Uit'.");
        }

        [MonitoredTest("Should have an age groupbox"), Order(4)]
        public void _4_ShouldHaveAnAgeGroupBox()
        {
            AssertHasAgeGroupBox();
            var stackPanel = _ageGroupBox.Content as StackPanel;
            Assert.That(stackPanel, Is.Not.Null, () => "The 'Content' of the 'GroupBox' should be a 'StackPanel' that can contain multiple child controls.");
            var checkboxes = stackPanel.Children.OfType<CheckBox>().ToList();
            Assert.That(checkboxes, Has.Count.EqualTo(3), () => "The 'StackPanel' in the 'GroupBox' should have 3 'CheckBox' controls.");
            Assert.That(checkboxes.First().IsChecked, Is.True, () => "The first 'CheckBox' should have its 'IsChecked' property set to true");
        }

        [MonitoredTest("Should have a gender groupbox"), Order(5)]
        public void _5_ShouldHaveAGenderGroupBox()
        {
            AssertHasGenderGroupBox();
            var stackPanel = _genderGroupBox.Content as StackPanel;
            Assert.That(stackPanel, Is.Not.Null, () => "The 'Content' of the 'GroupBox' should be a 'StackPanel' that can contain multiple child controls.");
            var checkboxes = stackPanel.Children.OfType<RadioButton>().ToList();
            Assert.That(checkboxes, Has.Count.EqualTo(2), () => "The 'StackPanel' in the 'GroupBox' should have 2 'RadioButton' controls.");
            Assert.That(checkboxes.First().IsChecked, Is.True, () => "The first 'RadioButton' should have its 'IsChecked' property set to true");
        }

        [MonitoredTest("Should have all controls in a canvas"), Order(6)]
        public void _6_ShouldHaveAllControlsInACanvas()
        {
            AssertHasToggleButton();
            AssertHasAgeGroupBox();
            AssertHasGenderGroupBox();
            Assert.That(_canvas, Is.Not.Null, () => "A 'Canvas' control could not be found");
            Assert.That(_toggleButton.Parent, Is.EqualTo(_canvas), () => "The 'ToggleButton' is not in the 'Canvas'.");
            Assert.That(_ageGroupBox.Parent, Is.EqualTo(_canvas), () => "The age 'GroupBox' is not in the 'Canvas'.");
            Assert.That(_genderGroupBox.Parent, Is.EqualTo(_canvas), () => "The gender 'GroupBox' is not in the 'Canvas'.");

            AssertUsesCanvasPositioning(_toggleButton, "ToggleButton");
            AssertUsesCanvasPositioning(_ageGroupBox, "age GroupBox");
            AssertUsesCanvasPositioning(_genderGroupBox, "gender GroupBox");
        }

        private void AssertHasToggleButton()
        {
            Assert.That(_toggleButton, Is.Not.Null, () => "A button of the type 'ToggleButton' could not be found.");
        }

        private void AssertHasAgeGroupBox()
        {
            Assert.That(_ageGroupBox, Is.Not.Null, () => "A groupbox with header equal to 'Leeftijd' could not be found.");
        }

        private void AssertHasGenderGroupBox()
        {
            Assert.That(_genderGroupBox, Is.Not.Null, () => "A groupbox with header equal to 'Geslacht' could not be found.");
        }

        private void AssertUsesCanvasPositioning(Control control, string controlName)
        {
            Assert.That(Canvas.GetLeft(control), Is.GreaterThan(0),
                () => ShowCanvasPositioningMesssage(controlName, "Canvas.Left"));
            Assert.That(Canvas.GetTop(control), Is.GreaterThan(0),
                () => ShowCanvasPositioningMesssage(controlName, "Canvas.Top"));

            Assert.That(control.Margin.Left == 0 && control.Margin.Top == 0, Is.True,
                () =>
                    $"The '{controlName}' has a 'Margin' set, but that is not necessary when positioning controls in a 'Canvas'. " +
                    "Use the attached properties 'Canvas.Left' and 'Canvas.Top' instead.");
        }

        private string ShowCanvasPositioningMesssage(string controlName, string attachedProperty)
        {
            return
                $"The '{controlName}' should use the attached property '{attachedProperty}' to position itself in the 'Canvas'.";
        }
    }
}
