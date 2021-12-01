using System.Collections.Generic;
using Lottery.Domain;

namespace Lottery.AppLogic.Interfaces
{
    public interface ILotteryGameRepository
    {
        IList<LotteryGame> GetAll();
    }
}