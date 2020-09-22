using System;
using System.Windows;

namespace Exercise4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ItWebBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ItWebBrowser.Source = ItWebBrowser.Visibility == Visibility.Visible
                ? new Uri("https://youtu.be/pScCoUb3BNY")
                : null;
        }

        private void ElectronicsWebBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ElectronicsWebBrowser.Source = ElectronicsWebBrowser.Visibility == Visibility.Visible
                ? new Uri("https://youtu.be/siywmpNvZNU")
                : null;
        }
    }
}