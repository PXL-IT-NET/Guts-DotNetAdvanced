using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise12.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise12", @"Exercise12\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private StackPanel _outerStackPanel;
        private GroupBox _languageGroupBox;
        private ListView _colorListView;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            _outerStackPanel = _window.GetUIElements<StackPanel>().FirstOrDefault();
            _languageGroupBox = _window.GetUIElements<GroupBox>().FirstOrDefault();
            _colorListView = _window.GetUIElements<ListView>().FirstOrDefault();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _1_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise12\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("B8-AF-26-9D-19-D5-02-97-0B-EA-63-F3-D2-27-63-93"), () => $"The file '{codeBehindFilePath}' has changed. " +
                                                                             "Undo your changes on the file to make this test pass. " +
                                                                             "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("Should have an outer StackPanel"), Order(2)]
        public void _2_ShouldHaveAnOuterStackPanel()
        {
            Assert.That(_outerStackPanel, Is.Not.Null,
                "Cannot find a 'StackPanel'.");
            Assert.That(_outerStackPanel.Parent, Is.SameAs(_window.Window),
                "The child of the 'Window' should be a 'StackPanel'.");
            Assert.That(_outerStackPanel.Orientation, Is.EqualTo(Orientation.Vertical),
                "The outer 'StackPanel' should have a vertical 'Orientation'.");
        }

        [MonitoredTest("Should have a language GroupBox"), Order(3)]
        public void _3_ShouldHaveALanguageGroupBox()
        {
            Assert.That(_languageGroupBox, Is.Not.Null, () => "A 'GroupBox' could not be found.");
            var header = _languageGroupBox.Header as string;
            Assert.That(header, Is.EqualTo("Favorite language").IgnoreCase,
                "The header of the 'GroupBox' should be 'Favorite language'.");

            Assert.That(HasMarginOnAllSides(_languageGroupBox.Margin), Is.True,
                "The 'GroupBox' should have some 'Margin' on all sides.");

            var stackPanel = _languageGroupBox.Content as StackPanel;
            Assert.That(stackPanel, Is.Not.Null,
                "The 'Content' of the 'GroupBox' should be a 'StackPanel' that can contain multiple child controls.");
            Assert.That(stackPanel.Orientation, Is.EqualTo(Orientation.Vertical),
                "The 'StackPanel' of the 'GroupBox' should have a vertical orientation.");
            var radioButtons = stackPanel.Children.OfType<RadioButton>().ToList();
            Assert.That(radioButtons, Has.Count.EqualTo(3),
                "The 'StackPanel' in the 'GroupBox' should have 3 'RadioButton' controls.");
            Assert.That(radioButtons.First().IsChecked, Is.True,
                "The first 'RadioButton' should have its 'IsChecked' property set to true.");
        }

        [MonitoredTest("Should have a color ListView"), Order(4)]
        public void _4_ShouldHaveAColorListView()
        {
            Assert.That(_colorListView, Is.Not.Null, () => "A 'ListView' could not be found.");

            Assert.That(HasMarginOnAllSides(_colorListView.Margin), Is.True,
                "The 'ListView' should have some 'Margin' on all sides.");

            var items = _colorListView.Items.OfType<ListViewItem>().ToList();
            Assert.That(items, Has.Count.GreaterThanOrEqualTo(3), "The 'ListView' should contain at least 3 instances of 'ListViewItem'.");

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                AssertCollorListViewItem(item, index);
            }
        }

        [MonitoredTest("Should have a style for all RadioButtons in the Window"), Order(5)]
        public void _5_ShouldHaveAStyleForAllRadioButtonsInTheWindow()
        {
            var style = _window.Window.Resources.Values.OfType<Style>().FirstOrDefault();
            Assert.That(style, Is.Not.Null,
                "Cannot find a 'Style' in the 'Window' 'Resources'.");

            Assert.That(style.TargetType, Is.EqualTo(typeof(RadioButton)), 
                "The 'Style' should target the 'RadioButton' type.");

            var isStyleForAllRadioButtons = _window.Window.Resources.Contains(typeof(RadioButton));
            Assert.That(isStyleForAllRadioButtons, Is.True, 
                "The 'Style' should be automatically applied on all 'RadioButtons' in the 'Window'. " +
                "You can achieve this by not specifying a key for the 'Style' resource.");

            var isCheckedTrigger = style.Triggers.OfType<Trigger>().FirstOrDefault();
            Assert.That(isCheckedTrigger, Is.Not.Null, 
                "The style should have a 'Trigger' in its 'Triggers' collection.");
            Assert.That(isCheckedTrigger.Property.Name, Is.EqualTo("IsChecked"),
                "The trigger should be for the 'Property' 'IsChecked'.");
            Assert.That(isCheckedTrigger.Value, Is.True, 
                "The trigger should be activated when the 'Value' of 'IsChecked' is 'True'.");

            var triggerBackgroundSetter = isCheckedTrigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property.Name.ToLower() == "background");
            Assert.That(triggerBackgroundSetter, Is.Not.Null,
                () =>
                    "No 'Setter' found in the trigger for the 'Background' property. " +
                    "When the trigger is activated a 'Setter' should set the 'Value' of the 'Background' property to 'Yellow'.");
            Assert.That(triggerBackgroundSetter.Value.ToString(), Contains.Substring("FFFF00").IgnoreCase,
                () => "The 'Value' property of the 'Setter' in the 'Trigger' for 'Background' should be set to 'Yellow'.");

            var triggerFontWeightSetter = isCheckedTrigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property.Name.ToLower() == "fontweight");
            Assert.That(triggerFontWeightSetter, Is.Not.Null,
                () =>
                    "No 'Setter' found in the trigger for the 'FontWeight' property. " +
                    "When the trigger is activated a 'Setter' should set the 'Value' of the 'FontWeight' property to 'Bold'.");
            Assert.That(triggerFontWeightSetter.Value.ToString(), Is.EqualTo("Bold").IgnoreCase,
                () => "The 'Value' property of the 'Setter' in the 'Trigger' for 'FontWeight' should be set to 'Bold'.");
        }

        private void AssertCollorListViewItem(ListViewItem item, int itemIndex)
        {
            var stackPanel = item.Content as StackPanel;
            Assert.That(stackPanel, Is.Not.Null,
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'Content' should be a 'StackPanel' that can contain multiple child controls.");
            Assert.That(stackPanel.Orientation, Is.EqualTo(Orientation.Horizontal),
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'StackPanel' should have a horizontal 'Orientation'.");

            var ellipse = stackPanel.Children.OfType<Ellipse>().FirstOrDefault();
            Assert.That(ellipse, Is.Not.Null,
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'StackPanel' should contain an 'Ellipse'.");
            Assert.That(ellipse.Fill, Is.Not.Null,
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'Ellipse' should have a 'Fill' set.");
            Assert.That(ellipse.Width, Is.GreaterThan(0),
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'Ellipse' should have a 'Width' set.");
            Assert.That(ellipse.Height, Is.GreaterThan(0),
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'Ellipse' should have a 'Height' set.");

            var textBlock = stackPanel.Children.OfType<TextBlock>().FirstOrDefault();
            Assert.That(textBlock, Is.Not.Null,
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'StackPanel' should contain a 'TextBlock'.");
            Assert.That(textBlock.Margin.Left, Is.GreaterThan(0),
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'TextBlock' should have some 'Margin' on the left.");
            Assert.That(textBlock.Text, Is.Not.Empty,
                $"'ListViewItem {itemIndex + 1}': " +
                "The 'TextBlock' should have a 'Text' set.");
        }

        private bool HasMarginOnAllSides(Thickness margin)
        {
            if (margin.Left <= 0) return false;
            if (margin.Top <= 0) return false;
            if (margin.Right <= 0) return false;
            if (margin.Bottom <= 0) return false;
            return true;
        }

    }
}
