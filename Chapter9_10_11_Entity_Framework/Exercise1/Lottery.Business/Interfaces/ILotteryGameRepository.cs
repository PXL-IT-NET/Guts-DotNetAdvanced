using System.Collections.Generic;
using Lottery.Domain;

namespace Lottery.Business.Interfaces
{
    public interface ILotteryGameRepository
    {
        IList<LotteryGame> GetAll();
    }
}