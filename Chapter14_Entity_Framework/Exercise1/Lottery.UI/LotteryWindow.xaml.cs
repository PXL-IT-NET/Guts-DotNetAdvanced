using System.Windows;
using System.Windows.Controls;
using Lottery.Business.Interfaces;
using Lottery.Data.Interfaces;
using Lottery.Domain;

namespace Lottery.UI
{
    public partial class LotteryWindow : Window
    {
        public LotteryWindow(ILotteryGameRepository lotteryGameRepository, 
            IDrawRepository drawRepository, IDrawService drawService)
        {
            InitializeComponent();
        }

        private void ShowDrawsButton_Click(object sender, RoutedEventArgs e)
        {
            //Do NOT change any code in this method

            RetrieveDraws();

            NewDrawButton.Visibility = Visibility.Visible;
            DrawsListView.Visibility = Visibility.Visible;
        }

        private void NewDrawButton_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO: create the draw

            RetrieveDraws(); //Refreshed the draws that are shown in the ListView
        }

        private void GameComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Do NOT change any code in this method

            NewDrawButton.Visibility = Visibility.Hidden;
            DrawsListView.Visibility = Visibility.Hidden;
        }

        private void RetrieveDraws()
        {
            //TODO: get the draws and show them
        }
    }
}
