using System.Windows;
using HeroApp.Business.Contracts;

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
