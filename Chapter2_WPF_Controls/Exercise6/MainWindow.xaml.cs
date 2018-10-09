using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Exercise6
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
       
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            Button btn =(Button) e.Source;
            numberTextBox.AppendText(btn.Content as string);

        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length==1)
            {
                e.Handled=!(char.IsDigit(char.Parse(e.Text)));
            }
            else
            {
                e.Handled = true;
            }
             
           
        }

      
    }
}
