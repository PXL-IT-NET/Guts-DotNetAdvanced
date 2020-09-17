using System.Windows;

namespace NumberConverter.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Number(0);
        }
    }
}
