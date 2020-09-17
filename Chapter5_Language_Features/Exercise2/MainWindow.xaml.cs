using System;
using System.Windows;

namespace Exercise2
{
    public partial class MainWindow : Window
    {
        public MainWindow(IMathOperationFactory operationFactory)
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
