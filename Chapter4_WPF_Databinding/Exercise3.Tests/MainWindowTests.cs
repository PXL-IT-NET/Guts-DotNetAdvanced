using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using TestUtils;

namespace Exercise3.Tests
{
    [ExerciseTestFixture("dotnet2", "H04", "Exercise03",
        @"Exercise3\MainWindow.xaml;Exercise3\MainWindow.xaml.cs;")]
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private ListView _listView;
        private GroupBox _newGameGroupBox;
        private TextBox _nameTextBox;
        private TextBox _descriptionTextBox;
        private Button _addNewGameButton;
        private TextBlock _errorMessageTextBlock;

        [SetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            _listView = _window.GetUIElements<ListView>().FirstOrDefault();

            _newGameGroupBox = _window.GetPrivateField<GroupBox>(f => f.Name == "NewGameGroupBox");
            _nameTextBox = _window.GetPrivateField<TextBox>(f => f.Name == "NameTextBox");
            _descriptionTextBox = _window.GetPrivateField<TextBox>(f => f.Name == "DescriptionTextBox");
            _addNewGameButton = _window.GetPrivateField<Button>(f => f.Name == "AddNewGameButton");
            _errorMessageTextBlock = _window.GetPrivateField<TextBlock>(f => f.Name == "ErrorMessageTextBlock");
        }

        [TearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed Game.cs"), Order(1)]
        public void _01_ShouldNotHaveChangedGameClass()
        {
            var hash = Solution.Current.GetFileHash(@"Exercise3\Game.cs");

            Assert.That(hash, Is.EqualTo("E4-86-8E-1E-65-E3-64-41-A5-3B-C8-FF-5B-11-96-B0"));
        }

        [MonitoredTest("The ListView source of data should be a collection that notifies of changes"), Order(2)]
        public void _02_ListView_SourceOfData_ShouldBeACollectionThatNotifiesOfChanges()
        {
            AssertHasListView();

            var collectionThatNotifiesChanges = _listView.ItemsSource as INotifyCollectionChanged;
            Assert.That(collectionThatNotifiesChanges, Is.Not.Null,
                "The source of data is not set or isn't of a type that notifies changes.");

            Assert.That(collectionThatNotifiesChanges, Is.InstanceOf<ObservableCollection<Game>>(),
                "The 'ObservableCollection' class implements 'INotifyCollectionChanged'. " +
                "There is no need use an own implementation.");

            var games = _listView.ItemsSource.OfType<Game>().ToList();
            Assert.That(games, Has.Count.GreaterThanOrEqualTo(2),
                "The source of data should contain at least 2 instances of 'Game'.");
        }

        [MonitoredTest("The ListView should have 2 columns"), Order(3)]
        public void _03_ListView_ShouldHave2Columns()
        {
            var columns = AssertAndGetListViewColumns();

            var columnHeaderErrorMessage = "One of the column headers is not correct.";
            Assert.That(columns,
                Has.One.Matches((GridViewColumn column) => (column.Header as string) == "Name"),
                columnHeaderErrorMessage);
            Assert.That(columns,
                Has.One.Matches((GridViewColumn column) => (column.Header as string) == "Description"),
                columnHeaderErrorMessage);

            var nameColumn = columns.First();
            Assert.That(nameColumn.Width, Is.EqualTo(double.NaN),
                "The width of the first column (Name) should adapt to its contents.");
            var descriptionColumn = columns.ElementAt(1);
            Assert.That(descriptionColumn.Width, Is.GreaterThanOrEqualTo(300),
                "The width of the second column (Description) should be at least 300.");
        }

        [MonitoredTest("The ListView columns should have correct bindings defined"), Order(4)]
        public void _04_ListView_Columns_ShouldHaveCorrectBindings()
        {
            var columns = AssertAndGetListViewColumns();

            var nameColumn = columns.First();
            var nameBinding = nameColumn.DisplayMemberBinding as Binding;
            var invalidNameBindingMessage = "The cells in de 'Name' column are not correctly bound to the name of the games.";
            Assert.That(nameBinding, Is.Not.Null, invalidNameBindingMessage);
            Assert.That(nameBinding.Path.Path, Is.EqualTo("Name"), invalidNameBindingMessage);
            Assert.That(nameBinding.Mode, Is.AnyOf(BindingMode.Default, BindingMode.OneWay), invalidNameBindingMessage);

            var descriptionColumn = columns.ElementAt(1);
            var mustUseTemplateForDescriptionMessage = "The cells in de 'Description' should use a custom template that contains a 'TextBlock' that has 'TextWrapping' set to 'Wrap'.";
            Assert.That(descriptionColumn.CellTemplate, Is.Not.Null, mustUseTemplateForDescriptionMessage);
            var textBlock = descriptionColumn.CellTemplate.LoadContent() as TextBlock;
            Assert.That(textBlock, Is.Not.Null, mustUseTemplateForDescriptionMessage);
            Assert.That(textBlock.TextWrapping, Is.EqualTo(TextWrapping.Wrap), mustUseTemplateForDescriptionMessage);
            BindingUtil.AssertBinding(textBlock, TextBlock.TextProperty, "Description", BindingMode.OneWay);

            Assert.That(descriptionColumn.DisplayMemberBinding, Is.Null,
                "Don't use 'DisplayMemberBinding' when a 'CellTemplate' is defined.");
        }

        [MonitoredTest("The game form should have correct bindings"), Order(5)]
        public void _05_GameForm_ShouldHaveCorrectBindings()
        {
            AssertHasFormControls();

            BindingUtil.AssertBinding(_nameTextBox, TextBox.TextProperty, "Name", BindingMode.TwoWay);
            BindingUtil.AssertBinding(_descriptionTextBox, TextBox.TextProperty, "Description", BindingMode.TwoWay);
        }

        [MonitoredTest("The game form should be bound to an empty game"), Order(6)]
        public void _06_GameForm_ShouldBeBoundToAnEmptyGame()
        {
            AssertHasFormControls();

            var game = _newGameGroupBox.DataContext as Game;
            Assert.That(game, Is.Not.Null, "No game 'DataContext' found for the 'GroupBox'.");

            var mustBeEmptyGameMessage = "The game should be empty (no name, no description).";
            Assert.That(game.Name, Is.Null.Or.Empty, mustBeEmptyGameMessage);
            Assert.That(game.Description, Is.Null.Or.Empty, mustBeEmptyGameMessage);
        }

        [MonitoredTest("Add game should show an error message for an invalid game"), Order(7)]
        public void _07_GameForm_AddGame_ShouldShowErrorMessageForInvalidGame()
        {
            var gameAdded = TryAddGame(string.Empty, Guid.NewGuid().ToString(), out Game addedGame);
            Assert.That(gameAdded, Is.False, "A game with an empty name can be added.");
            Assert.That(_errorMessageTextBlock.Text, Is.Not.Empty,
                "No error message is shown when adding a game with an empty name.");

            gameAdded = TryAddGame(Guid.NewGuid().ToString(), string.Empty, out addedGame);
            Assert.That(gameAdded, Is.False, "A game with an empty description can be added.");
            Assert.That(_errorMessageTextBlock.Text, Is.Not.Empty,
                "No error message is shown when adding a game with an empty description.");
        }

        [MonitoredTest("Add game should add a valid game to the games collection"), Order(8)]
        public void _08_GameForm_AddGame_ShouldAddValidGameToTheGamesCollection()
        {
            AssertHasFormControls();

            var gameToAdd = new Game
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString()
            };

            var gameAdded = TryAddGame(gameToAdd.Name, gameToAdd.Description, out Game addedGame);
            var failMessage = "A valid game is not added correctly.";
            Assert.That(gameAdded, Is.True, failMessage);
            Assert.That(addedGame.Name, Is.EqualTo(gameToAdd.Name), failMessage);
            Assert.That(addedGame.Description, Is.EqualTo(gameToAdd.Description), failMessage);
        }

        [MonitoredTest("The game form should be bound to a new empty game after adding one"), Order(9)]
        public void _09_GameForm_ShouldBeBoundToANewEmtpyGameAfterAddingOne()
        {
            var gameToAdd = new Game
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString()
            };

            var gameAdded = TryAddGame(gameToAdd.Name, gameToAdd.Description, out Game addedGame);
            Assert.That(gameAdded, Is.True, "A valid game is not added correctly.");

            var nextNewGame = _newGameGroupBox.DataContext as Game;
            var failMessage = "The 'DataContext' of the 'GroupBox' is not set to a new empty game.";
            Assert.That(nextNewGame, Is.Not.Null, failMessage);
            Assert.That(nextNewGame, Is.Not.SameAs(addedGame), failMessage);
            Assert.That(nextNewGame.Name, Is.Null.Or.Empty, failMessage);
            Assert.That(nextNewGame.Description, Is.Null.Or.Empty, failMessage);
        }

        [MonitoredTest("The game form should clear previous error message after adding a game"), Order(10)]
        public void _10_GameForm_ShouldClearPreviousErrorMessageAfterAddingAGame()
        {
            AssertHasFormControls();

            _errorMessageTextBlock.Text = "A previous error message";
            _08_GameForm_AddGame_ShouldAddValidGameToTheGamesCollection();
            Assert.That(_errorMessageTextBlock.Text, Is.Null.Or.Empty,
                "Previous error messages are not cleared when the add operation succeeds.");
        }

        [MonitoredTest("The description TextBox should be configured correctly"), Order(11)]
        public void _11_DescriptionTextBox_ShouldBeConfiguredCorrectly()
        {
            AssertHasFormControls();

            Assert.That(_descriptionTextBox.TextWrapping, Is.EqualTo(TextWrapping.Wrap), "Text must be wrapped.");
            Assert.That(_descriptionTextBox.AcceptsReturn, Is.True, "It must be possible to type multiple lines of text.");

            var invalidLanguageMessage = "The language for spell checking should be flemish (belgian dutch).";
            Assert.That(_descriptionTextBox.Language, Is.Not.Null, invalidLanguageMessage);
            Assert.That(_descriptionTextBox.Language.IetfLanguageTag, Is.EqualTo("nl-be").IgnoreCase, invalidLanguageMessage);

            Assert.That(SpellCheck.GetIsEnabled(_descriptionTextBox), Is.True, "Spell checking must be enabled.");
        }

        [MonitoredTest("Should use a ScrollViewer"), Order(12)]
        public void _12_ShouldUseAScrollViewer()
        {
            var scrollViewer = _window.GetUIElements<ScrollViewer>().FirstOrDefault();

            Assert.That(scrollViewer, Is.Not.Null, "Cannot find a 'ScrollViewer'.");
            Assert.That(scrollViewer.Parent, Is.SameAs(_window.Window), "The 'ScrollViewer' should be a direct child of the main window.");
        }

        private void AssertHasFormControls()
        {
            Assert.That(_newGameGroupBox, Is.Not.Null, "Could not find the 'GroupBox' with name 'NewGameGroupBox'.");
            Assert.That(_nameTextBox, Is.Not.Null, "Could not find the 'TextBox' with name 'NameTextBox'.");
            Assert.That(_descriptionTextBox, Is.Not.Null, "Could not find the 'TextBox' with name 'DescriptionTextBox'.");
            Assert.That(_addNewGameButton, Is.Not.Null, "Could not find the 'Button' with name 'AddNewGameButton'.");
            Assert.That(_errorMessageTextBlock, Is.Not.Null, "Could not find the 'TextBlock' with name 'ErrorMessageTextBlock'.");
        }

        private void AssertHasListView()
        {
            Assert.That(_listView, Is.Not.Null, "Cannot find a 'ListView'. Have you deleted it?");
        }

        private GridViewColumnCollection AssertAndGetListViewColumns()
        {
            AssertHasListView();

            var invalidListViewConfigurationMessage = "The columns of the 'ListView' are not defined correctly.";
            var gridView = _listView.View as GridView;
            Assert.That(gridView, Is.Not.Null, invalidListViewConfigurationMessage);
            Assert.That(gridView.Columns, Has.Count.EqualTo(2), invalidListViewConfigurationMessage);
            return gridView.Columns;
        }

        private bool TryAddGame(string name, string description, out Game addedGame)
        {
            addedGame = null;
            AssertHasFormControls();
            AssertHasListView();

            AssertDoesNotReadTextBoxPropertiesInClickEventHandler();

            var originalGames = GetGamesFromListView();
            var game = _newGameGroupBox.DataContext as Game;
            Assert.That(game, Is.Not.Null, "No game 'DataContext' found for the 'GroupBox'.");
            game.Name = name;
            game.Description = description;
            _addNewGameButton.FireClickEvent();

            var games = GetGamesFromListView();
            if (games.Count == originalGames.Count + 1)
            {
                addedGame = games.Last();
                return true;
            }

            return false;
        }

        private IList<Game> GetGamesFromListView()
        {
            var games = new List<Game>();
            if (_listView.ItemsSource != null)
            {
                games = _listView.ItemsSource.OfType<Game>().ToList();
            }
            return games;
        }


        private void AssertDoesNotReadTextBoxPropertiesInClickEventHandler()
        {
            var code = Solution.Current.GetFileContent(@"Exercise3\MainWindow.xaml.cs");
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            var clickEventHandlerMethods = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(md =>
                {
                    var parameters = md.ParameterList.Parameters;
                    if (parameters.Count != 2) return false;
                    if (!(parameters[0] is ParameterSyntax senderParameter)) return false;
                    if (senderParameter.Type.ToString().ToLower() != "object")
                    {
                        return false;
                    }

                    if (!(parameters[1] is ParameterSyntax eventArgsParameter)) return false;
                    if (eventArgsParameter.Type.ToString() != "RoutedEventArgs")
                    {
                        return false;
                    }

                    return true;
                }).ToList();

            Assert.That(clickEventHandlerMethods, Has.Count.LessThanOrEqualTo(1),
                "Your code contains more than one click event handler.");

            Assert.That(clickEventHandlerMethods, Has.Count.EqualTo(1), "Cannot find a click event handler.");

            var addNewGameButtonClickMethod = clickEventHandlerMethods.First();

            var textPropertyReads = addNewGameButtonClickMethod.Body.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .Where(ma => ma.ToString().ToLower().Contains("textbox") && ma.Name.ToString() == "Text" && !(ma.Parent is AssignmentExpressionSyntax)).ToList();

            Assert.That(textPropertyReads, Is.Empty,
                "You should not read the Text property of the name or description TextBox. " +
                "Since you are using two-way data binding you can access the data source object directly.");
        }
    }
}