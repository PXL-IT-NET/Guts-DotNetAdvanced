using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using HeroApp.Business.Contracts;
using HeroApp.Domain.Contracts;
using HeroApp.UI;
using Moq;
using NUnit.Framework;

namespace HeroApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise01",
        @"HeroApp.UI\MainWindow.xaml;HeroApp.UI\MainWindow.xaml.cs;HeroApp.UI\App.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private MainWindow _window;
        private Mock<IBattleService> _battleServiceMock;
        private IBattle _battle;
        private Button _fightRoundButton;
        private Mock<IBattle> _battleMock;

        [SetUp]
        public void BeforeEachTest()
        {
            _battleMock = new Mock<IBattle>();
            _battle = _battleMock.Object;
            _battleServiceMock = new Mock<IBattleService>();
            _battleServiceMock.Setup(service => service.SetupRandomBattle()).Returns(_battle);

            _window = new MainWindow(_battleServiceMock.Object);
            _window.Show();

            _fightRoundButton = _window.GetPrivateFieldValueByName<Button>("FightRoundButton");
        }

        [TearDown]
        public void AfterEachTest()
        {
            _window.Close();
        }

        [MonitoredTest("MainWindow - Constructor - Should create a random battle and set it as DataContext")]
        public void Constructor_ShouldCreateARandomBattleAndSetItAsDataContext()
        {
            _battleServiceMock.Verify(service => service.SetupRandomBattle(), Times.Once,
                "The 'SetupRandomBattle' method of the battle service should be used.");

            Assert.That(_window.DataContext, Is.SameAs(_battle),
                "The battle returned by the battle service should be the DataContext of the window.");
        }

        [MonitoredTest("MainWindow - Should have correct bindings defined")]
        public void ShouldHaveCorrectBindingsDefined()
        {
            Grid grid = _window.Content as Grid;
            IList<StackPanel> stackPanels = grid.FindVisualChildren<StackPanel>().ToList();

            Assert.That(stackPanels.Count, Is.EqualTo(2), "There should be exactly 2 StackPanel elements in the window.");

            for (int fighterNumber = 1; fighterNumber <= 2; fighterNumber++)
            {
                StackPanel stackPanel = stackPanels[fighterNumber - 1];
                IList<TextBlock> textBlocks = stackPanel.FindVisualChildren<TextBlock>().ToList();
                Assert.That(textBlocks.Count, Is.EqualTo(2),
                    "There should be exactly 2 TextBlock elements " +
                    $"that are children of the StackPanel of fighter {fighterNumber}.");

                TextBlock nameTextBlock = textBlocks[0];
                AssertBinding(nameTextBlock, $"fighter {fighterNumber} name TextBlock", TextBlock.TextProperty,
                    $"Fighter{fighterNumber}.Name", BindingMode.OneWay);

                TextBlock healthTextBlock = textBlocks[1];
                AssertBinding(healthTextBlock, $"fighter {fighterNumber} health TextBlock", TextBlock.TextProperty,
                    $"Fighter{fighterNumber}.Health", BindingMode.OneWay);
            }
        }

        [MonitoredTest("MainWindow - Fight round click - Should call FightRound on battle")]
        public void FightRoundButtonClick_ShouldCallFightRoundOnBattle()
        {
            _fightRoundButton.FireClickEvent();

            _battleMock.Verify(battle => battle.FightRound(), Times.Once,
                "FightRound method is not called on battle created by the service.");
        }

        [MonitoredTest("MainWindow - Fight round click - BattleIsOver - Should disable button")]
        public void FightRoundButtonClick_BattleIsOver_ShouldDisableButton()
        {
            _battleMock.SetupGet(battle => battle.IsOver).Returns(true);
            Assert.That(_fightRoundButton.IsEnabled, Is.True, "The button is not disabled before the click.");
            _fightRoundButton.FireClickEvent();
            _battleMock.Verify(battle => battle.FightRound(), Times.Once,
                "FightRound method is not called on battle created by the service.");
            Assert.That(_fightRoundButton.IsEnabled, Is.False, "The button is not disabled after the click.");
        }

        private void AssertBinding(FrameworkElement targetElement, string targetName, DependencyProperty targetProperty,
            string expectedBindingPath, BindingMode allowedBindingMode)
        {
            BindingExpression binding = targetElement.GetBindingExpression(targetProperty);
            var errorMessage =
                $"Invalid 'Binding' for the '{targetProperty.Name}' property of {targetName}.";
            Assert.That(binding, Is.Not.Null, errorMessage);
            Assert.That(binding.ParentBinding.Path.Path, Is.EqualTo(expectedBindingPath), errorMessage);

            var allowedBindingModes = new List<BindingMode> { allowedBindingMode };
            var metaData = (FrameworkPropertyMetadata)targetProperty.GetMetadata(targetElement);
            if (allowedBindingMode == BindingMode.TwoWay && metaData.BindsTwoWayByDefault)
            {
                allowedBindingModes.Add(BindingMode.Default);
            }
            else if (allowedBindingMode == BindingMode.OneWay)
            {
                allowedBindingModes.Add(BindingMode.Default);
            }
            Assert.That(allowedBindingModes, Has.One.EqualTo(binding.ParentBinding.Mode), errorMessage);
        }
    }
}
