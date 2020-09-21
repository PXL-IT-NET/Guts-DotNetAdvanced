using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Exercise4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PXLITWebBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(PXLITWebBrowser.Source is null)
            {
                PXLITWebBrowser.Source = new Uri("https://youtu.be/pScCoUb3BNY");
            }
            else
            {
                PXLITWebBrowser.Source = null;
            }
        }

        private void PXLEAWebBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PXLEAWebBrowser.Source is null)
            {
                PXLEAWebBrowser.Source = new Uri("https://youtu.be/siywmpNvZNU");
            }
            else
            {
                PXLEAWebBrowser.Source = null;
            }
        }
        


 
    }
}
