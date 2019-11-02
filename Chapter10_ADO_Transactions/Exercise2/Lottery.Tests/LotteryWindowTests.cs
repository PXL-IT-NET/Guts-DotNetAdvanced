using System;
using Guts.Client.Shared.TestTools;
using Lottery.Business.Interfaces;
using Lottery.Data.Interfaces;
using Lottery.Domain;
using Lottery.UI;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Lottery.UI.Converters;

namespace Lottery.Tests
{
    [ExerciseTestFixture("dotnet2", "H10", "Exercise02",
        @"Lottery.Data\ConnectionFactory.cs;
Lottery.Data\LotteryGameRepository.cs;
Lottery.Data\DrawRepository.cs;
Lottery.Business\DrawService.cs;
Lottery.UI\LotteryWindow.xaml;
Lottery.UI\LotteryWindow.xaml.cs;
Lottery.UI\Converters\DrawNumbersConverter.cs;")]
    [Apartment(ApartmentState.STA)]
    public class LotteryWindowTests
    {
        private LotteryWindow _window;
        private Mock<ILotteryGameRepository> _lotterGameRepositoryMock;
        private Mock<IDrawRepository> _drawRepositoryMock;
        private Mock<IDrawService> _drawServiceMock;
        private ComboBox _gameCombobox;
        private List<LotteryGame> _allGames;
        private DatePicker _fromDatePicker;
        private DatePicker _untilDatePicker;
        private Button _showDrawsButton;
        private ListView _drawsListView;
        private Button _newDrawButton;

        [SetUp]
        public void Setup()
        {
            _lotterGameRepositoryMock = new Mock<ILotteryGameRepository>();
            _allGames = new List<LotteryGame> { new LotteryGameBuilder().Build() };
            _lotterGameRepositoryMock.Setup(repo => repo.GetAll()).Returns(_allGames);

            _drawRepositoryMock = new Mock<IDrawRepository>();
            _drawServiceMock = new Mock<IDrawService>();
        }

        [TearDown]
        public void TearDown()
        {
            _window?.Close();
        }

        [MonitoredTest("LotteryWindow - Should load the lottery games on construction"), Order(1)]
        public void _01_ShouldLoadTheLotteryGamesOnConstruction()
        {
            //Act
            InitializeWindow(_lotterGameRepositoryMock.Object, _drawRepositoryMock.Object, _drawServiceMock.Object);

            //Assert
            _lotterGameRepositoryMock.Verify(repo => repo.GetAll(), Times.Once,
                "The 'GetAll' method of the 'lotteryGameRepository' was not called.");
            Assert.That(_gameCombobox.ItemsSource, Is.EqualTo(_allGames),
                () =>
                    "The list of games returned by the repository should be assigned to the 'ItemsSource' of the combobox.");

            Assert.That(_gameCombobox.SelectedIndex, Is.EqualTo(0),
                () => "The first item of the combobox should be selected.");

        }

        [MonitoredTest("LotteryWindow - Should show draws of the selected game when the ShowDrawsButton is clicked"), Order(2)]
        public void _02_ShouldShowDrawsOfSelectedGameOnShowDrawsButtonClick()
        {
            //Arrange
            InitializeWindow(_lotterGameRepositoryMock.Object, _drawRepositoryMock.Object, _drawServiceMock.Object);
            _gameCombobox.SelectedIndex = 0;
            var selectedGame = _allGames[0];
            var fromDate = DateTime.Now.AddDays(-10);
            var untilDate = DateTime.Now.AddDays(-1);
            _fromDatePicker.SelectedDate = fromDate;
            _untilDatePicker.SelectedDate = untilDate;

            var foundDraws = new List<Draw>();

            _drawRepositoryMock.Setup(repo => repo.Find(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(foundDraws);

            //Act
            _showDrawsButton.FireClickEvent();

            //Assert
            _drawRepositoryMock.Verify(repo => repo.Find(selectedGame.Id, fromDate, untilDate), Times.Once,
                "The 'Find' method of the draw repository is not called correctly.");

            Assert.That(_drawsListView.ItemsSource, Is.EqualTo(foundDraws),
                () =>
                    "The list of draws found by the repository should be assigned to the 'ItemsSource' property of the 'ListView'.");
        }

        [MonitoredTest("LotteryWindow - Should use an ItemTemplate to display each draw with its numbers"), Order(3)]
        public void _03_ShouldUseAnItemTemplateToDisplayEachDrawWithItsNumbers()
        {
            //Arrange
            InitializeWindow(_lotterGameRepositoryMock.Object, _drawRepositoryMock.Object, _drawServiceMock.Object);

            //Assert
            Assert.That(_drawsListView.ItemTemplate, Is.TypeOf<DataTemplate>(),
                () => "The 'ItemTemplate' property of the 'ListView' should be an instance of 'DataTemplate'.");

            var templateStackPanel = _drawsListView.ItemTemplate.LoadContent() as StackPanel;
            Assert.That(templateStackPanel, Is.Not.Null,
                () => "The 'DataTemplate' of the 'ItemTemplate' should contain a 'StackPanel'.");
            Assert.That(templateStackPanel.Orientation, Is.EqualTo(Orientation.Horizontal),
                () => "The 'StackPanel' in the 'DataTemplate' should be horizontal.");
            var textBlocks = templateStackPanel.Children.OfType<TextBlock>().ToList();
            Assert.That(textBlocks.Count, Is.EqualTo(2),
                () => "The 'StackPanel' in the 'DataTemplate' should contain 2 instances of 'TextBlock'.");

            var dateTextBlock = textBlocks[0];
            BindingExpression binding = dateTextBlock.GetBindingExpression(TextBlock.TextProperty);
            Assert.That(binding, Is.Not.Null, () => "Could not find a 'Binding' for the 'Text' property of the date TextBlock.");
            Assert.That(binding.ParentBinding.Path.Path, Is.EqualTo("Date"),
                () =>
                    "The source property of the databinding statement of the date TextBlock should be the 'Date' property of the source object (Draw).");
            Assert.That(binding.ParentBinding.StringFormat, Is.EqualTo("dd/MM/yyyy HH:mm"),
                () => "The 'StringFormat' for the binding of the date TextBlock should be 'dd/MM/yyyy HH:mm'.");

            var drawNumbersTextBlock = textBlocks[1];
            binding = drawNumbersTextBlock.GetBindingExpression(TextBlock.TextProperty);
            Assert.That(binding, Is.Not.Null, () => "Could not find a 'Binding' for the 'Text' property of the draw numbers TextBlock.");
            Assert.That(binding.ParentBinding.Path.Path, Is.EqualTo("DrawNumbers"),
                () =>
                    "The source property of the databinding statement of the draw numbers TextBlock should be the 'DrawNumbers' property of the source object (Draw).");
            Assert.That(binding.ParentBinding.Converter, Is.TypeOf<DrawNumbersConverter>(),
                () => "The 'Converter' for the binding of the draw numbers TextBlock should be an instance of 'DrawNumbersConverter'.");
        }

        [MonitoredTest("LotteryWindow - Should create a new draw when the NewDrawButton is clicked"), Order(4)]
        public void _04_ShouldCreateANewDrawOnNewDrawButtonClick()
        {
            //Arrange
            InitializeWindow(_lotterGameRepositoryMock.Object, _drawRepositoryMock.Object, _drawServiceMock.Object);
            _gameCombobox.SelectedIndex = 0;
            var selectedGame = _allGames[0];

            //Act
            _newDrawButton.FireClickEvent();

            //Assert
            _drawServiceMock.Verify(service => service.CreateDrawFor(selectedGame), Times.Once,
                "The 'CreateDrawFor' method of the draw service is not called correctly.");
        }

        private void InitializeWindow(ILotteryGameRepository lotteryGameRepository, IDrawRepository drawRepository, IDrawService drawService)
        {
            _window = new LotteryWindow(lotteryGameRepository, drawRepository, drawService);
            _window.Show();

            _gameCombobox = _window.GetPrivateFieldValue<ComboBox>();
            _fromDatePicker = _window.GetPrivateFieldValueByName<DatePicker>("FromDatePicker");
            _untilDatePicker = _window.GetPrivateFieldValueByName<DatePicker>("UntilDatePicker");
            _showDrawsButton = _window.GetPrivateFieldValueByName<Button>("ShowDrawsButton");
            _newDrawButton = _window.GetPrivateFieldValueByName<Button>("NewDrawButton");
            _drawsListView = _window.GetPrivateFieldValueByName<ListView>("DrawsListView");
        }
    }
}