using System.Collections.Generic;
using Lottery.Domain;

namespace Lottery.Data.Interfaces
{
    public interface ILotteryGameRepository
    {
        IList<LotteryGame> GetAll();
    }
}