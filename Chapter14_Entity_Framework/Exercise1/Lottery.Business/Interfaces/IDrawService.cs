using Lottery.Domain;

namespace Lottery.Business.Interfaces
{
    public interface IDrawService
    {
        void CreateDrawFor(LotteryGame lotteryGame);
    }
}