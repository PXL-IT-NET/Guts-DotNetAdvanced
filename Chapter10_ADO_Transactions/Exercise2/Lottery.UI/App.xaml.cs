using System.Windows;
using Lottery.Business;
using Lottery.Data;

namespace Lottery.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var connectionFactory = new ConnectionFactory();

            var lotteryGameRepository = new LotteryGameRepository(connectionFactory);
            var drawRepository = new DrawRepository(connectionFactory);
            var drawService = new DrawService(drawRepository);
            var lotteryWindow = new LotteryWindow(lotteryGameRepository, drawRepository, drawService);

            lotteryWindow.Show();
        }
    }
}
