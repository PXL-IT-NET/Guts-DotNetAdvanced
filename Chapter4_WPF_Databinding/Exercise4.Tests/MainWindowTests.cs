using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Exercise4.Converters;

namespace Exercise4.Tests
{
    [ExerciseTestFixture("dotnet2", "H04", "Exercise04", 
        @"Exercise4\MainWindow.xaml;Exercise4\Converters\BooleanToVisibilityConverter.cs;")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private WebBrowser _itBrowser, _electronicsBrowser;
        private RadioButton _itRadioButton;
        private RadioButton _electronicsRadioButton;

        [SetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            IList<WebBrowser> webBrowsers = _window.GetUIElements<WebBrowser>();
            _itBrowser = webBrowsers[0];
            _electronicsBrowser = webBrowsers[1];
            IList<RadioButton> radioButtons = _window.GetUIElements<RadioButton>();
            _itRadioButton = radioButtons[0];
            _electronicsRadioButton = radioButtons[1];
        }

        [TearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file"), Order(1)]
        public void _01_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise4\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("B0-50-0E-F1-C1-41-40-4C-76-D5-30-7F-AC-48-CD-B6"),
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass. " +
                "This exercise can be completed by purely working with XAML.");
        }

        [MonitoredTest("Should have a Boolean2VisibilityConverter in Window resources"), Order(2)]
        public void _02_ShouldHaveABoolean2VisibilityConverterInWindowResources()
        {
            var booleanToVisibilityConverter = _window.Window.Resources.Values.OfType<Boolean2VisibilityConverter>().FirstOrDefault();
            Assert.That(booleanToVisibilityConverter, Is.Not.Null, () => "The window should contain exactly one resource of type 'Boolean2VisibilityConverter'.");
        }

        [MonitoredTest("WebBrowsers visibility should bind to RadioButtons Checked property"), Order(3)]
        public void _03_WebBrowsersVisibilityShouldBindToRadioButtonsCheckedProperty()
        {
            AssertBindsToRadioButton(_itBrowser, _itRadioButton, "IT WebBrowser");
            AssertBindsToRadioButton(_electronicsBrowser, _electronicsRadioButton, "Electronics WebBrowser");
        }

        [MonitoredTest("Should use the converter in the binding of the WebBrowsers"), Order(4)]
        public void _04_ShouldUseTheConverterInBindingOfTheWebBrowsers()
        {
            AssertUsesBooleanToVisibilityConverter(_itBrowser, "IT WebBrowser");
            AssertUsesBooleanToVisibilityConverter(_electronicsBrowser, "Electronics WebBrowser");
        }

        [MonitoredTest("Should toggle WebBrowsers visibility when a RadioButton is clicked"), Order(5)]
        public void _05_ShouldToggleWebBrowsersVisibilityWhenARadioButtonIsClicked()
        {
            _itRadioButton.IsChecked = true;

            Assert.That(_itBrowser.Visibility, Is.EqualTo(Visibility.Visible),
                "The IT WebBrowser Should be visible when you click the IT RadioButton");
            Assert.That(_electronicsBrowser.Visibility, Is.EqualTo(Visibility.Hidden),
                "The Electronics WebBrowser should be hidden when you click the IT RadioButton");

            _electronicsRadioButton.IsChecked = true;

            Assert.That(_itBrowser.Visibility, Is.EqualTo(Visibility.Hidden),
                "The IT WebBrowser should be hidden when you click the Electronics RadioButton");
            Assert.That(_electronicsBrowser.Visibility, Is.EqualTo(Visibility.Visible),
                "The Electronics WebBrowser should be visible when you click the Electronics RadioButton");
        }

        private static BindingExpression GetAndAssertVisibilityBinding(WebBrowser browser, string browserName)
        {
            BindingExpression visibilityBinding = browser.GetBindingExpression(UIElement.VisibilityProperty);
            Assert.That(visibilityBinding, Is.Not.Null, $"No binding found for the 'Visibility' property of '{browserName}'.");
            return visibilityBinding;
        }

        private void AssertBindsToRadioButton(WebBrowser browser, RadioButton radioButton, string browserName)
        {
            BindingExpression visibilityBinding = GetAndAssertVisibilityBinding(browser, browserName);

            //ElementName
            string elementName = visibilityBinding.ParentBinding.ElementName;
            Assert.That(elementName, Is.Not.Null.Or.Empty,
                $"The {browserName} should use an 'ElementName' in its binding statement.");

            Assert.That(elementName, Is.EqualTo(radioButton.Name),
                $"The {browserName} should use '{radioButton.Name}' as data source.");

            //Path
            string path = visibilityBinding.ParentBinding.Path.Path;
            Assert.That(path, Is.EqualTo("IsChecked"),
                $"The {browserName} should use the 'IsChecked' property of the data source (RadioButton) in its binding statement.");

        }

        private void AssertUsesBooleanToVisibilityConverter(WebBrowser browser, string browserName)
        {
            BindingExpression visibilityBinding = GetAndAssertVisibilityBinding(browser, browserName);
            Assert.That(visibilityBinding.ParentBinding.Converter, Is.Not.Null,
                 $"The {browserName} should use the BooleanToVisibilityConverter in its binding statement.");
            Assert.That(visibilityBinding.ParentBinding.Converter, Is.TypeOf<Boolean2VisibilityConverter>(),
                $"The {browserName} should use the BooleanToVisibilityConverter in its binding statement.");
        }
    }
}