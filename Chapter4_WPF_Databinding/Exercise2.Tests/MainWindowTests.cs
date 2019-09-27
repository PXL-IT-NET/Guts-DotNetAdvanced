using Guts.Client.Classic.TestTools.WPF;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Exercise2.Converters;
using Guts.Client.Classic;
using Guts.Client.Shared;
using TestUtils;

namespace Exercise2.Tests
{
    [ExerciseTestFixture("dotnet2", "H04", "Exercise02",
         @"Exercise2\MainWindow.xaml;Exercise2\MainWindow.xaml.cs;Exercise2\Game.cs;Exercise2\Converters\RatingConverter.cs")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private TextBox _gameIdTextBox;
        private TextBox _nameTextBox;
        private TextBox _typeTextBox;
        private TextBox _releaseDateTextBox;
        private TextBox _descriptionTextBox;
        private TextBox _ratingTextBox;

        private ComboBox _gameComboBox;

        private CheckBox _ageCheckBox;

        private Button _upButton;
        private Button _downButton;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            var allTextBoxes = _window.GetUIElements<TextBox>().OrderBy(textbox => textbox.Margin.Top).ToList();
            if (allTextBoxes.Count >= 1)
            {
                _gameIdTextBox = allTextBoxes.ElementAt(0);
            }

            if (allTextBoxes.Count >= 2)
            {
                _nameTextBox = allTextBoxes.ElementAt(1);
            }

            if (allTextBoxes.Count >= 3)
            {
                _typeTextBox = allTextBoxes.ElementAt(2);
            }

            if (allTextBoxes.Count >= 4)
            {
                _releaseDateTextBox = allTextBoxes.ElementAt(3);
            }

            if (allTextBoxes.Count >= 5)
            {
                _descriptionTextBox = allTextBoxes.ElementAt(4);
            }

            if (allTextBoxes.Count >= 6)
            {
                _ratingTextBox = allTextBoxes.ElementAt(5);
            }


            _gameComboBox = _window.GetUIElements<ComboBox>().FirstOrDefault();

            _ageCheckBox = _window.GetUIElements<CheckBox>().FirstOrDefault();

            var allButtons = _window.GetUIElements<Button>().OrderBy(button => button.Margin.Left).ToList();

            if (allButtons.Count >= 1)
            {
                _upButton = allButtons.ElementAt(0);
            }

            if (allTextBoxes.Count >= 2)
            {
                _downButton = allButtons.ElementAt(1);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should have six TextBoxes,a CheckBox and two Buttons"), Order(1)]
        public void _01_ShouldHaveSixTextBoxesACheckBoxAndTwoButtons()
        {
            AssertHasSixTextBoxesACheckBoxAndTwoButtons();
        }

        [MonitoredTest("Should have a ComboBox with an ItemsSource of 7 games"), Order(2)]
        public void _02_ShouldHaveAComboBoxWithAnItemsSourceOf7Games()
        {
            AssertHasComboBox();
            Assert.That(_gameComboBox.ItemsSource, Is.InstanceOf<IEnumerable<Game>>(),
                () => "The 'ItemsSource' property should be an IEnumerable of games.");
            Assert.That(_gameComboBox.Items.Count, Is.EqualTo(7),
                () => "The 'ItemsSource' property should contain exactly 7 games.");
            Assert.That(_gameComboBox.ItemTemplate, Is.Not.Null,
                () => "The ComboBox should use an 'ItemTemplate' to display 'GameId - Name'.");
            var templateStackPanel = _gameComboBox.ItemTemplate.LoadContent() as StackPanel;
            Assert.That(templateStackPanel, Is.Not.Null,
                () => "The 'DataTemplate' of the 'ItemTemplate' should contain a 'StackPanel'.");
            Assert.That(templateStackPanel.Orientation, Is.EqualTo(Orientation.Horizontal),
                () => "The 'StackPanel' in the 'DataTemplate' should be horizontal.");
            Assert.That(templateStackPanel.Children.OfType<TextBlock>().Count(), Is.EqualTo(3),
                () => "The 'StackPanel' in the 'DataTemplate' should contain 3 instances of 'TextBlock'.");
        }

        [MonitoredTest("Should have a DataContext set to the selected game after game selection"), Order(3)]
        public void _03_ShouldHaveDataContextSetToSelectedGameAfterGameSelection()
        {
            _02_ShouldHaveAComboBoxWithAnItemsSourceOf7Games();
            var games = _gameComboBox.ItemsSource.OfType<Game>().ToList();
            for (int i = 0; i < games.Count; i++)
            {
                AssertGameSelection(games, i);
            }
        }

        [MonitoredTest("Should have correct bindings for the TextBoxes and the CheckBox"), Order(4)]
        public void _04_ShouldHaveCorrectBindingsForTheTextBoxesAndTheCheckBox()
        {
            AssertHasSixTextBoxesACheckBoxAndTwoButtons();

            BindingUtil.AssertBinding(_gameIdTextBox, TextBox.TextProperty, "GameId", BindingMode.TwoWay);
            BindingUtil.AssertBinding(_nameTextBox, TextBox.TextProperty, "Name", BindingMode.TwoWay);
            BindingUtil.AssertBinding(_typeTextBox, TextBox.TextProperty, "Type", BindingMode.TwoWay);
            BindingUtil.AssertBinding(_releaseDateTextBox, TextBox.TextProperty, "ReleaseDate", BindingMode.TwoWay);
            BindingUtil.AssertBinding(_descriptionTextBox, TextBox.TextProperty, "Description", BindingMode.TwoWay);
            BindingUtil.AssertBinding(_ageCheckBox, ToggleButton.IsCheckedProperty, "IsUnder18", BindingMode.TwoWay);
        }

        [MonitoredTest("Should implement INotifyPropertyChanged interface in Game class"), Order(5)]
        public void _05_ShouldImplementINotifyPropertyChangedInterfaceInGameClass()
        {
            var game = new Game { Rating = 0.5d };

            var notifier = game as INotifyPropertyChanged;
            Assert.That(notifier, Is.Not.Null, () => "The Game class should implement the INotifyPropertyChanged interface");

            var hasChangeNotificationForRating = false;
            notifier.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == "Rating")
                {
                    hasChangeNotificationForRating = true;
                }
            };
            game.Rating += 0.1;

            Assert.That(hasChangeNotificationForRating, Is.True,
                () => "The 'PropertyChanged' event is not triggered when the 'Rating' property is changed.");
        }

        [MonitoredTest("Should have a RateConverter in Window resources"), Order(6)]
        public void _06_ShouldHaveARateConverterInWindowResources()
        {
            var ratingConverter = _window.Window.Resources.Values.OfType<RatingConverter>().FirstOrDefault();
            Assert.That(ratingConverter, Is.Not.Null, () => "The window should contain exactly one resource of type 'RatingConverter'.");
        }

        [MonitoredTest("Should use the RatingConverter in the binding of the RatingTextBox"), Order(7)]
        public void _07_ShouldUseRateConverterInBindingOfRatingTextBox()
        {
            var converter = AssertUsesRatingConverterInBindingAndGetTheConverter();
            Assert.That(converter, Is.TypeOf<RatingConverter>(),
                () => "The rating TextBox should use the RatingConverter in its binding statement.");
        }

        [MonitoredTest("Should multiply rating by 10 in Convert method"), Order(8)]
        public void _08_ShouldMultiplyRatingBy10InConvertMethod()
        {
            var converter = AssertUsesRatingConverterInBindingAndGetTheConverter();
            Assert.That(converter.Convert(1.0, null, null, null), Is.EqualTo(10),
                () => "Convert should multiply by 10");
        }

        [MonitoredTest("Should divide rating by 10 in ConvertBack method"), Order(9)]
        public void _09_ShouldDivideRatingBy10InConvertBackMethod()
        {
            var converter = AssertUsesRatingConverterInBindingAndGetTheConverter();
            Assert.That(converter.ConvertBack(10, null, null, null), Is.EqualTo(1.0), () => "ConvertBack should divide by 10");
        }

        [MonitoredTest("Should rate up when UpButton is clicked"), Order(10)]
        public void _10_ShouldRateUpWhenUpButtonIsClicked()
        {
            AssertHasSixTextBoxesACheckBoxAndTwoButtons();
            AssertHasComboBox();

            _gameComboBox.SelectedIndex = 0;
            Game game = (Game)_window.Window.DataContext;
            double rating = game.Rating;
            _upButton.FireClickEvent();
            Assert.That(game.Rating, Is.EqualTo(rating + 0.10),
                () => "The up button should add 0.10 to the rating value.");
        }

        [MonitoredTest("Should rate down when DownButton is clicked"), Order(11)]
        public void _11_ShouldRateDownWhenDownButtonIsClicked()
        {
            AssertHasSixTextBoxesACheckBoxAndTwoButtons();
            AssertHasComboBox();

            _gameComboBox.SelectedIndex = 0;
            Game game = (Game)_window.Window.DataContext;
            double rating = game.Rating;
            _downButton.FireClickEvent();
            Assert.That(game.Rating, Is.EqualTo(rating - 0.10),
                () => "The up button should substract 0.10 from the rating value.");
        }

        private void AssertHasSixTextBoxesACheckBoxAndTwoButtons()
        {
            Assert.That(_gameIdTextBox, Is.Not.Null, () => "The textbox for the game Id could not be found.");
            Assert.That(_nameTextBox, Is.Not.Null, () => "The textbox for the name could not be found.");
            Assert.That(_typeTextBox, Is.Not.Null, () => "The textbox for the type could not be found.");
            Assert.That(_releaseDateTextBox, Is.Not.Null, () => "The textbox for the release date could not be found.");
            Assert.That(_descriptionTextBox, Is.Not.Null, () => "The textbox for the description could not be found.");
            Assert.That(_ratingTextBox, Is.Not.Null, () => "The textbox for the rating could not be found.");

            Assert.That(_ageCheckBox, Is.Not.Null, () => "The checkbox for the age could not be found.");

            Assert.That(_upButton, Is.Not.Null, () => "The up button could not be found.");
            Assert.That(_downButton, Is.Not.Null, () => "The down button could not be found.");
        }

        private void AssertHasComboBox()
        {
            Assert.That(_gameComboBox.ItemsSource, Is.Not.Null,
                () => "The 'ItemsSource' property of the combobox is not set.");
            Assert.That(_gameComboBox.ItemsSource, Is.Not.Empty,
                () => "The 'ItemsSource' property of the combobox should not be empty.");
        }

        private void AssertGameSelection(List<Game> games, int selectedIndex)
        {
            _gameComboBox.SelectedIndex = selectedIndex;
            Assert.That(_window.Window.DataContext, Is.TypeOf<Game>(),
                () => "The Datacontext of the window should be a Game object.");
            Assert.That(_window.Window.DataContext, Is.EqualTo(games[_gameComboBox.SelectedIndex]),
                () => $"When game {selectedIndex + 1} is selected, it is not set as DataContext of the window.");
        }


        private IValueConverter AssertUsesRatingConverterInBindingAndGetTheConverter()
        {
            BindingExpression ratingBinding = _ratingTextBox.GetBindingExpression(TextBox.TextProperty);
            Assert.That(ratingBinding, Is.Not.Null, () => "No binding found for the rating TextBox.");
            Assert.That(ratingBinding.ParentBinding.Converter, Is.Not.Null,
                () => "The rating TextBox should use the RatingConverter in its binding statement.");
            return ratingBinding.ParentBinding.Converter;
        }
    }
}