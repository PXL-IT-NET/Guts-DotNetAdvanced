using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Exercise2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var operationFactory = new MathOperationFactory();
            var mainWindow = new MainWindow(operationFactory);
            mainWindow.Show();
        }
    }
}
