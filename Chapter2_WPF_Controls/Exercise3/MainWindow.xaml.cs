using System;
using System.Windows;

namespace Exercise3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void growButton_Click(object sender, RoutedEventArgs e)
        {
            if (myCanvas.Width - 30 >= orangeRectangle.Width)
            {
                orangeRectangle.Width += 10;
            }
        }

        private void shrinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (10 < orangeRectangle.Width)
            {
                orangeRectangle.Width -= 10;
            }
        }
    }
}
