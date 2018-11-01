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

        private void Grow(object sender, RoutedEventArgs e)
        {
            if (oranje_rectangle.Height + 10 >= canvas.Height || oranje_rectangle.Width + 10 >= canvas.Width)
            {
                MessageBox.Show("De rechthoek kan niet groter. Probeer de shrink button");
            }
            else
            {
                oranje_rectangle.Height += 10;
                oranje_rectangle.Width += 10;

            }
            
        }


        private void Shrink(object sender, RoutedEventArgs e)
        {
            if (oranje_rectangle.Height - 10  < 0 || oranje_rectangle.Width - 10 < 0)
            {
                MessageBox.Show("De rechthoek kan niet groter. Probeer de shrink button");
            }
            else
            {
                oranje_rectangle.Height -= 10;
                oranje_rectangle.Width -= 10;

            }
        }
    }
}
