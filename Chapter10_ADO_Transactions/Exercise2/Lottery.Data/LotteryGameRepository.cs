using System.Collections.Generic;
using Lottery.Data.Interfaces;
using Lottery.Domain;

namespace Lottery.Data
{
    public class LotteryGameRepository : ILotteryGameRepository
    {
        public LotteryGameRepository(IConnectionFactory connectionFactory)
        {
        }

        public IList<LotteryGame> GetAll()
        {
            return null;
        }
    }
}