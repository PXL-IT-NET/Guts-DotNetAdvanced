using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;

namespace Exercise4.Tests
{
    [ExerciseTestFixture("dotnet2", "H04", "Exercise04", @"Exercise4\MainWindow.xaml;Exercise4\MainWindow.xaml.cs;")]
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private IList<WebBrowser> _webBrowsers;
        private WebBrowser _itBrowser, _eaBrowser;
        private IList<RadioButton> _radioButtons;

        [SetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            _webBrowsers = _window.GetUIElements<WebBrowser>();
            _itBrowser = _webBrowsers[0];
            _eaBrowser = _webBrowsers[1];
            _radioButtons = _window.GetUIElements<RadioButton>();

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
            Assert.That(fileHash, Is.EqualTo("7A-42-80-16-38-CF-5A-F2-0E-75-3B-ED-0F-8D-AB-E9"),
                () =>
                    $"The file '{codeBehindFilePath}' has changed. " +
                    "Undo your changes on the file to make this test pass. " +
                    "This exercise can be completed by purely working with XAML.");
        }      

        [MonitoredTest("Should have a BooleanToVisibilityConverter in Window resources"), Order(2)]
        public void _02_ShouldHaveABooleanToVisibilityConverterInWindowResources()
        {
            var booleanToVisibilityConverter = _window.Window.Resources.Values.OfType<Exercise4.Converters.BooleanToVisibilityConverter>().FirstOrDefault();
            Assert.That(booleanToVisibilityConverter, Is.Not.Null, () => "The window should contain exactly one resource of type 'BooleanToVisibilityConverter'.");
        }

        [MonitoredTest("Should use the BooleanToVisibilityConverter in the binding of the Webbrowsers"), Order(3)]
        public void _03_ShouldUseBooleanToVisibilityConverterInBindingOfTheWebBrowsers()
        {
            var iTconverter = AssertUsesBooleanToVisibilityConvertInTheBindingAndGetTheConverter(_itBrowser);
            Assert.That(iTconverter, Is.TypeOf<Exercise4.Converters.BooleanToVisibilityConverter>(),
                () => "The IT WebBrowser should use the BooleanToVisibilityConverter in its binding statement.");
            var eAconverter = AssertUsesBooleanToVisibilityConvertInTheBindingAndGetTheConverter(_eaBrowser);
            Assert.That(eAconverter, Is.TypeOf<Exercise4.Converters.BooleanToVisibilityConverter>(),
                () => "The EA WebBrowser should use the BooleanToVisibilityConverter in its binding statement.");
        }        

        [MonitoredTest("Should use ConverterParameter in the binding of the WebBrowsers"), Order(4)]
        public void _04_ShouldUseConverterParameterInBindingOfTheWebBrowsers()
        {
            var iTconverterParameter = AssertUsesBooleanToVisibilityConvertInTheBindingAndGetTheConverterParameter(_itBrowser);
            Assert.That(iTconverterParameter, Is.EqualTo("Visible|Hidden"),
                () => "The IT WebBrowser should use the BooleanToVisibilityConverter in its binding statement with a ConvertParameter.");
            var eAconverterParameter = AssertUsesBooleanToVisibilityConvertInTheBindingAndGetTheConverterParameter(_eaBrowser);
            Assert.That(eAconverterParameter, Is.EqualTo("Visible|Hidden"),
                () => "The EA WebBrowser should use the BooleanToVisibilityConverter in its binding statement with a ConvertParameter.");
        }

        [MonitoredTest("Should use ConverterParameter in the binding of the WebBrowsers"), Order(5)]
        public void _05_ShouldUseElementNameInTheBindingOfTheWebBrowsers()
        {
            var itElementName = AssertUsesBooleanElementNameInTheBindingAndGetTheElementName(_itBrowser);
            Assert.That(itElementName, Is.EqualTo("itRadio"),
                () => "The IT WebBrowser should use the 'itRadio' ElementName in its binding statement with a ConvertParameter.");
            var eAconverterParameter = AssertUsesBooleanElementNameInTheBindingAndGetTheElementName(_eaBrowser);
            Assert.That(eAconverterParameter, Is.EqualTo("eaRadio"),
                () => "The EA WebBrowser should use the 'eaRadio' ElementName in its binding statement with a ConvertParameter.");
        }

        [MonitoredTest("Should use Path in the binding of the WebBrowsers"), Order(6)]
        public void _06_ShouldUsePathInTheBindingOfTheWebBrowsers()
        {
            var itPath = AssertUsesPathInTheBindingAndGetThePath(_itBrowser);
            Assert.That(itPath, Is.EqualTo("IsChecked"),
                () => "The IT WebBrowser should use the 'IsChecked' Path in its binding statement with a ConvertParameter.");
            var eaPath = AssertUsesPathInTheBindingAndGetThePath(_eaBrowser);
            Assert.That(eaPath, Is.EqualTo("IsChecked"),
                () => "The EA WebBrowser should use the 'IsChecked' Path in its binding statement with a ConvertParameter.");
        }

        [MonitoredTest("Should rate down when DownButton is clicked"), Order(7)]
        public void _07_ShouldHideItBrowserAndShowEaBrowserWhenEaRadioButtonIsClicked()
        {
            RadioButton itRadio = _radioButtons.FirstOrDefault(r => r.Name == "itRadio");
            RadioButton eaRadio = _radioButtons.FirstOrDefault(r => r.Name == "eaRadio");
           
            itRadio.IsChecked = true;
           
            Assert.That(_itBrowser.Visibility, Is.EqualTo(System.Windows.Visibility.Visible),
                () => "The IT Web Browser Should be visible when you click the IT Radio Button");
            Assert.That(_eaBrowser.Visibility, Is.EqualTo(System.Windows.Visibility.Hidden),
                () => "The EA Web Browser Should be hidden when you click the IT Radio Button");

            eaRadio.IsChecked = true;

            Assert.That(_itBrowser.Visibility, Is.EqualTo(System.Windows.Visibility.Hidden),
                () => "The IT Web Browser Should be hidden when you click the EA Radio Button");
            Assert.That(_eaBrowser.Visibility, Is.EqualTo(System.Windows.Visibility.Visible),
                () => "The EA Web Browser Should be visible when you click the EA Radio Button");
        }

        private object AssertUsesPathInTheBindingAndGetThePath(WebBrowser browser)
        {
            BindingExpression visibilityBinding = browser.GetBindingExpression(WebBrowser.VisibilityProperty);
            Assert.That(visibilityBinding, Is.Not.Null, () => "No binding found for the IT WebBrowser.");
            Assert.That(visibilityBinding.ParentBinding.Path.Path, Is.Not.Null,
                                () => "The WebBrowsers should use the Path in its binding statement.");
            return visibilityBinding.ParentBinding.Path.Path;
        }

        private object AssertUsesBooleanElementNameInTheBindingAndGetTheElementName(WebBrowser browser)
        {
            BindingExpression visibilityBinding = browser.GetBindingExpression(WebBrowser.VisibilityProperty);
            Assert.That(visibilityBinding, Is.Not.Null, () => "No binding found for the IT WebBrowser.");
            Assert.That(visibilityBinding.ParentBinding.ElementName, Is.Not.Null,
                                () => "The WebBrowsers should use the ElementName in its binding statement.");
            return visibilityBinding.ParentBinding.ElementName;
        }

        private object AssertUsesBooleanToVisibilityConvertInTheBindingAndGetTheConverter(WebBrowser browser)
        {
            BindingExpression visibilityBinding = browser.GetBindingExpression(WebBrowser.VisibilityProperty);
            Assert.That(visibilityBinding, Is.Not.Null, () => "No binding found for the IT WebBrowser.");
            Assert.That(visibilityBinding.ParentBinding.Converter, Is.Not.Null,
                () => "The WebBrowsers should use the BooleanToVisibilityConverter in its binding statement.");
            return visibilityBinding.ParentBinding.Converter;
        }    

        private object AssertUsesBooleanToVisibilityConvertInTheBindingAndGetTheConverterParameter(WebBrowser browser)
        {
            BindingExpression visibilityBinding = browser.GetBindingExpression(WebBrowser.VisibilityProperty);
            Assert.That(visibilityBinding, Is.Not.Null, () => "No binding found for the EA WebBrowser.");
            Assert.That(visibilityBinding.ParentBinding.ConverterParameter, Is.Not.Null,
                () => "The IT WebBrowser should use the BooleanToVisibilityConverter in its binding statement and use a ConverterParameter.");
            return visibilityBinding.ParentBinding.ConverterParameter;
        }        
    }
}