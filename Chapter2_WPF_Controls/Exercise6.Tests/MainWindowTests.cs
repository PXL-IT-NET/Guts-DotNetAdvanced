using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;

namespace Exercise6.Tests
{
    [ExerciseTestFixture("dotNet2", "H02", "Exercise06", 
        @"Exercise6\MainWindow.xaml;Exercise6\MainWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private TextBox _numberTextBox;
        private List<Button> _digitButtons;


        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            _numberTextBox = _window.GetUIElements<TextBox>().FirstOrDefault();
            _digitButtons = _window.GetUIElements<Button>().ToList();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should have a textbox and 10 digit buttons"), Order(1)]
        public void _1_ShouldHaveATextBoxAnd10DigitButtons()
        {
            AssertHasAllControls();
        }

        [MonitoredTest("Should handle all button click events once by using event bubbling"), Order(2)]
        public void _2_ShouldHandleAllButtonClickEventsOnceByUsingEventBubbling()
        {
            AssertHasAllControls();

            var xamlCode = Solution.Current.GetFileContent(@"Exercise6\MainWindow.xaml");

            var clickHandlerForButtonRegex = new Regex(@"<Button\s.*Click\s?=\s?"".*>");
            Assert.That(clickHandlerForButtonRegex.IsMatch(xamlCode), Is.False,
                () =>
                    "There is a 'Click' handler defined for at least one 'Button'. " +
                    "Use event bubbling to catch the click events in the parent 'StackPanel'.");

            Assert.That(_digitButtons, Has.All.Matches((Button button) => string.IsNullOrEmpty(button.Name)),
                () =>
                    "There are one or more instances of 'Button' that have their 'x:Name' set in the XAML. " +
                    "This exercise can be made without naming the buttons. " +
                    "Use event bubbling to achieve this.");

            var clickHandlerForStackPanelRegex = new Regex(@"<StackPanel\s.*\.Click\s?=\s?"".*>");
            Assert.That(clickHandlerForStackPanelRegex.IsMatch(xamlCode), Is.True,
                () =>
                    "The 'StackPanel' containing the buttons should have defined which method handles bubbled click events from the buttons. " +
                    "You are expected to link the event handler in XAML code. " +
                    "Tip: 'ButtonBase.Click'.");
        }

        [MonitoredTest("Should append digits to the textbox when buttons are clicked"), Order(3)]
        public void _3_ShouldAppendDigitsToTheTextBoxWhenButtonsAreClicked()
        {
            AssertHasAllControls();

            _numberTextBox.Text = "";

            string expected = "";
            foreach (var digitButton in _digitButtons)
            {
                var buttonText = digitButton.Content as string;
                expected += buttonText;

                digitButton.FireClickEvent();

                Assert.That(_numberTextBox.Text, Is.EqualTo(expected));
            }
        }

        [MonitoredTest("Should allow appending digits to the textbox when typing"), Order(4)]
        public void _4_ShouldAllowAppendingDigitsToTheTextBoxWhenTyping()
        {
            AssertHasAllControls();

            var firedEventArgs = FirePreviewTextInputEvent("1");

            Assert.That(firedEventArgs.Handled, Is.False,
                () =>
                    "You should allow the TextInput tunnel event (PreviewTextInput) to reach the inner workings of the 'TextBox' control when a digit is typed.");
        }

        [MonitoredTest("Should avoid appending non-digits to the textbox when typing"), Order(5)]
        public void _5_ShouldAvoidAppendingNonDigitsToTheTextBoxWhenTyping()
        {
            AssertHasAllControls();

            var firedEventArgs = FirePreviewTextInputEvent("A");

            Assert.That(firedEventArgs.Handled, Is.True,
                () =>
                    "You should prevent TextInput tunnel event (PreviewTextInput) to reach the inner workings of the 'TextBox' control when a non-digit is typed. ");

            firedEventArgs = FirePreviewTextInputEvent("1A1");

            Assert.That(firedEventArgs.Handled, Is.True,
                () =>
                    "The 'PreviewTextInput' is handled correctly when the 'Text' property of the 'TextCompositionEventArgs' contains one (non-digit) character. " +
                    "But when the 'Text' is e.g. '1A1' the TextInput tunnel event is not prevented to reach the inner workings of the 'TextBox' control.");
        }

        private TextCompositionEventArgs FirePreviewTextInputEvent(string inputText)
        {
            var textComposition = new TextComposition(InputManager.Current, Keyboard.FocusedElement, inputText);
            var textCompositionEventArgs =
                new TextCompositionEventArgs(InputManager.Current.PrimaryKeyboardDevice, textComposition)
                {
                    RoutedEvent = TextCompositionManager.PreviewTextInputEvent
                };
            _numberTextBox.RaiseEvent(textCompositionEventArgs);

            return textCompositionEventArgs;
        }

        private void AssertHasAllControls()
        {
            Assert.That(_numberTextBox, Is.Not.Null, () => "No 'TextBox' could be found.");
            Assert.That(_digitButtons, Has.Count.EqualTo(10), () => "There should be 10 instances of 'Button' present.");
            foreach (var digitButton in _digitButtons)
            {
                var errorMessage = "All the buttons should have a single digit set as 'Content'.";
                var buttonText = digitButton.Content as string;
                Assert.That(buttonText, Is.Not.Null, () => errorMessage);
                Assert.That(buttonText.Length, Is.EqualTo(1), () => errorMessage);
                Assert.That(char.IsDigit(buttonText[0]), Is.True, () => errorMessage);
            }
        }
    }
}