using System;
using System.Diagnostics;
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

            parentStackPanel.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(btn_Click));
        }


        private void btn_Click (object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.Source;
            numberTextBox.Text += btn.Content;
            e.Handled = true;
        }

      

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            String keyPressedString = e.Text;
            int lengteKeyPressedString = keyPressedString.Length;
            char[] keyPressedArray = keyPressedString.ToCharArray();
            char keyPressed = keyPressedArray[lengteKeyPressedString - 1];
            bool charIsDigit = Char.IsDigit(keyPressed);
            if (!charIsDigit)
            {
                e.Handled = true;
            }
        }
    }
}
