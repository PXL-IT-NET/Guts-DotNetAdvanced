using System.Windows;

namespace HeroApp.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow window = new MainWindow(null);
            window.Show();
        }
    }
}
