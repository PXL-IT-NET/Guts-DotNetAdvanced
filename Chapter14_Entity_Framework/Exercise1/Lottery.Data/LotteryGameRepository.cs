using System.Collections.Generic;
using System.Linq;
using Lottery.Data.Interfaces;
using Lottery.Domain;

namespace Lottery.Data
{
    public class LotteryGameRepository : ILotteryGameRepository
    {
        public LotteryGameRepository(LotteryContext context)
        {
        }

        public IList<LotteryGame> GetAll()
        {
            return null;
        }
    }
}