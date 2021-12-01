using Lottery.Domain;

namespace Lottery.AppLogic.Interfaces
{
    public interface IDrawService
    {
        void CreateDrawFor(LotteryGame lotteryGame);
    }
}