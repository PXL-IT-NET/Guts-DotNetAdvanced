using System.Windows;
using Lottery.Business;
using Lottery.Data;

namespace Lottery.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = new LotteryContext();
            context.CreateOrUpdateDatabase();

            var lotteryGameRepository = new LotteryGameRepository(context);
            var drawRepository = new DrawRepository(context);
            var drawService = new DrawService(drawRepository);
            var lotteryWindow = new LotteryWindow(lotteryGameRepository, drawRepository, drawService);

            lotteryWindow.Show();
        }
    }
}
