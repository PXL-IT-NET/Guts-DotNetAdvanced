using System.Windows;
using HeroApp.AppLogic.Contracts;

namespace HeroApp.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(IBattleService battleService)
        {
            InitializeComponent();
        }
    }
}
