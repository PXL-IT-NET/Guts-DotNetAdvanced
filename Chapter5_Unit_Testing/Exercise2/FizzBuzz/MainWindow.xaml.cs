using System;
using System.Windows;
using FizzBuzz.Business;

namespace FizzBuzz
{
    public partial class MainWindow : Window
    {
        private readonly IFizzBuzzService _fizzBuzzService;

        public MainWindow(IFizzBuzzService fizzBuzzService)
        {
            InitializeComponent();

            _fizzBuzzService = fizzBuzzService;

            fizzTextBox.Text = "3";
            buzzTextBox.Text = "5";
            lastNumberTextBox.Text = "100";
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            int fizzFactor = 0;
            int buzzFactor = 0;
            int lastNumber = 0;

            try
            {
                fizzFactor = int.Parse(fizzTextBox.Text);
                buzzFactor = int.Parse(buzzTextBox.Text);
                lastNumber = int.Parse(lastNumberTextBox.Text);
            }
            catch (FormatException)
            {
                resultTextBlock.Text = "One of the numbers is invalid.";
            }
            catch (OverflowException)
            {
                resultTextBlock.Text = "One of the numbers is too big or too small.";
            }

            try
            {
                resultTextBlock.Text = _fizzBuzzService.GenerateFizzBuzzText(fizzFactor, buzzFactor, lastNumber);
            }
            catch (FizzBuzzValidationException validationException)
            {
                resultTextBlock.Text = validationException.Message;
            }
        }
    }
}
