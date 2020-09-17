using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Moq;
using NUnit.Framework;

namespace Exercise2.Tests
{
    [ExerciseTestFixture("dotnet2", "H5", "Exercise02", @"Exercise2\MainWindow.xaml.cs")]
    [RequiresThread(ApartmentState.STA)]
    public class MainWindowTests
    {
        private MainWindow _window;
        private TextBox _inputTextBox;
        private Button _startButton;
        private RadioButton _cubicRadioButton;
        private RadioButton _nthPrimeRadioButton;
        private ProgressBar _calculationProgressBar;
        private TextBlock _outputTextBlock;
        private Mock<IMathOperationFactory> _mathOperationFactoryMock;
        private CalculationWorker _calculationWorker;
        private List<CalculationEventArgs> _receivedEventArgs;
        private int _progressValueChangedCount;

        [SetUp]
        public void BeforeEachTest()
        {
            _mathOperationFactoryMock = new Mock<IMathOperationFactory>();
            _mathOperationFactoryMock.Setup(factory => factory.CreateCubicOperation()).Returns(n => 1);
            _mathOperationFactoryMock.Setup(factory => factory.CreateNthPrimeOperation()).Returns(n => -1);

            _window = new MainWindow(_mathOperationFactoryMock.Object);
            _window.Show();

            _inputTextBox = _window.GetPrivateFieldValueByName<TextBox>("inputTextBox");
            _startButton = _window.GetPrivateFieldValueByName<Button>("startButton");
            _cubicRadioButton = _window.GetPrivateFieldValueByName<RadioButton>("cubicRadioButton");
            _nthPrimeRadioButton = _window.GetPrivateFieldValueByName<RadioButton>("nthPrimeRadioButton");
            _calculationProgressBar = _window.GetPrivateFieldValueByName<ProgressBar>("calculationProgressBar");
            _outputTextBlock = _window.GetPrivateFieldValueByName<TextBlock>("outputTextBlock");

            try
            {
                _calculationWorker = _window.GetPrivateFieldValue<CalculationWorker>();
            }
            catch (Exception)
            {
                _calculationWorker = null;
            }

            _receivedEventArgs = new List<CalculationEventArgs>();
            _progressValueChangedCount = 0;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("MainWindow - Should have initialized readonly fields")]
        public void ShouldHaveInitializedReadOnlyFields()
        {
            var assembly = Assembly.GetAssembly(typeof(MainWindow));
            var mainWindowTypeInfo = assembly.DefinedTypes.First(t => t.AsType() == typeof(MainWindow));

            FieldInfo factoryFieldInfo = mainWindowTypeInfo.DeclaredFields.FirstOrDefault(f => f.FieldType == typeof(IMathOperationFactory));
            Assert.That(factoryFieldInfo, Is.Not.Null,
                "Cannot find a field (class variable) of type 'IMathOperationFactory'.");
            Assert.That(factoryFieldInfo.IsPrivate, Is.True, $"The field {factoryFieldInfo.Name} should be private.");
            Assert.That(factoryFieldInfo.IsInitOnly, Is.True, $"The field {factoryFieldInfo.Name} should be readonly.");

            IMathOperationFactory factoryValue = (IMathOperationFactory) factoryFieldInfo.GetValue(_window);
            Assert.That(factoryValue, Is.Not.Null, $"The field {factoryFieldInfo.Name} should be initialized in the constructor.");

            FieldInfo workerFieldInfo = mainWindowTypeInfo.DeclaredFields.FirstOrDefault(f => f.FieldType == typeof(CalculationWorker));
            Assert.That(workerFieldInfo, Is.Not.Null,
                "Cannot find a field (class variable) of type 'CalculationWorker'.");
            Assert.That(workerFieldInfo.IsPrivate, Is.True, $"The field {workerFieldInfo.Name} should be private.");
            Assert.That(workerFieldInfo.IsInitOnly, Is.True, $"The field {workerFieldInfo.Name} should be readonly.");

            CalculationWorker calculationWorkerValue = (CalculationWorker)workerFieldInfo.GetValue(_window);
            Assert.That(calculationWorkerValue, Is.Not.Null, $"The field {workerFieldInfo.Name} should be initialized in the constructor.");
        }

        [MonitoredTest("MainWindow - StartButton click when Cubic operation is checked - Should execute Cubic operation correctly")]
        public void StartButtonClick_CubicOperationChecked_ShouldExecuteCubicOperationCorrectly()
        {
            _inputTextBox.Text = "1 2 3";
            _cubicRadioButton.IsChecked = true;
            _outputTextBlock.Text = string.Empty;

            SubscribeToCalculationCompleteEvent();

            _calculationProgressBar.ValueChanged += ProgressBar_ValueChanged;

            //Act
            _startButton.FireClickEvent();
            DispatcherUtil.DoEvents(); //wait for the event handler calls to complete

            //Assert
            _mathOperationFactoryMock.Verify(factory => factory.CreateCubicOperation(), Times.Once,
                "The 'CreateCubicOperation' of the factory passed in the constructor is not called");
            Assert.That(_receivedEventArgs, Has.Count.EqualTo(3),
                "The CalculationCompleted handler method should be invoked 3 times when input is '1 2 3'.");
            Assert.That(_outputTextBlock.Text, Is.EqualTo("1 1 1"),
                "The 'outputTextBlock' should contain the returned results of the operation created by the 'operationFactory'.");
            Assert.That(_progressValueChangedCount, Is.EqualTo(3),
                "The value of 'calculationProgressBar' should have been changed 3 times.");
            Assert.That(_calculationProgressBar.Value, Is.EqualTo(1.0),
                "The value 'calculationProgressBar' should be 1.0 after all events are processed.");
        }

        [MonitoredTest("MainWindow - StartButton click when nth prime operation is checked - Should execute nth Prime operation correctly")]
        public void StartButtonClick_NthPrimeOperationChecked_ShouldExecuteNthPrimeOperationCorrectly()
        {
            _inputTextBox.Text = "1 2 3";
            _nthPrimeRadioButton.IsChecked = true;
            _outputTextBlock.Text = string.Empty;

            SubscribeToCalculationCompleteEvent();

            _calculationProgressBar.ValueChanged += ProgressBar_ValueChanged;

            //Act
            _startButton.FireClickEvent();
            DispatcherUtil.DoEvents(); //wait for the event handler calls to complete

            //Assert
            _mathOperationFactoryMock.Verify(factory => factory.CreateNthPrimeOperation(), Times.Once,
                "The 'CreateNthPrimeOperation' of the factory passed in the constructor is not called");
            Assert.That(_receivedEventArgs, Has.Count.EqualTo(3),
                "The CalculationCompleted handler method should be invoked 3 times when input is '1 2 3'.");
            Assert.That(_outputTextBlock.Text, Is.EqualTo("-1 -1 -1"),
                "The 'outputTextBlock' should contain the returned results of the operation created by the 'operationFactory'.");
            Assert.That(_progressValueChangedCount, Is.EqualTo(3),
                "The value of 'calculationProgressBar' should have been changed 3 times.");
            Assert.That(_calculationProgressBar.Value, Is.EqualTo(1.0),
                "The value 'calculationProgressBar' should be 1.0 after all events are processed.");
        }

        [MonitoredTest("MainWindow - StartButton click - Should reset progressbar and output when they have values from a previous click")]
        public void StartButtonClick_OutputAndProgressSetBecauseOfPreviousClick_ShouldResetProgressBarAndOutput()
        {
            _inputTextBox.Text = "";
            _outputTextBlock.Text = "Previous output";
            _calculationProgressBar.Value = 0.5;

            //Act
            _startButton.FireClickEvent();
            DispatcherUtil.DoEvents(); //wait for the event handler calls to complete

            //Assert
            Assert.That(_outputTextBlock.Text, Is.Empty,
                "The 'outputTextBlock' should be cleared.");
            Assert.That(_calculationProgressBar.Value, Is.Zero,
                "The value 'calculationProgressBar' should be 0.");
        }

        private void SubscribeToCalculationCompleteEvent()
        {
            EventInfo eventInfo = EventHelper.AssertAndRetrieveEventInfo();
            Assert.That(_calculationWorker, Is.Not.Null,
                "No private field of the type 'CalculationWorker' is found or the field is null after the window is constructed.");
            eventInfo.AddEventHandler(_calculationWorker, new CalculationCompleteHandler(CalculationCompleted));
        }

        private void CalculationCompleted(object sender, CalculationEventArgs args)
        {
            _receivedEventArgs.Add(args);
        }

        private void ProgressBar_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            _progressValueChangedCount++;
        }
    }
}