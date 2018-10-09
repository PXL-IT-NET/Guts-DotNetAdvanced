using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Exercise3
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            rectangle.Width = 80;
        }

      

       

        private void GrowButton_Click(object sender, RoutedEventArgs e)
        {
            if ((rectangle.Width + 10) < paperCanvas.Width)
            {
                rectangle.Width += 10;
            }
            else
            {
                rectangle.Width = paperCanvas.Width;
                Canvas.SetLeft(rectangle,0);
               
            }
           
        }

        private void ShrinkButton_Click(object sender, RoutedEventArgs e)
        {
            if ((rectangle.Width - 10) >= 0)
            {
                rectangle.Width -= 10;
            }
            else
            {
                rectangle.Width = 0;
            }
            
           

        }
    }
}
