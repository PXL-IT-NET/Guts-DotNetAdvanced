using System.Windows;
using FizzBuzz.Business;

namespace FizzBuzz
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = new MainWindow(new FizzBuzzService());
            mainWindow.Show();
        }
    }
}
