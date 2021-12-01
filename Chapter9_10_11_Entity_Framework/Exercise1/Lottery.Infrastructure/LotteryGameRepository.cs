using System;
using System.Collections.Generic;
using Lottery.AppLogic.Interfaces;
using Lottery.Domain;

namespace Lottery.Infrastructure
{
    internal class LotteryGameRepository : ILotteryGameRepository
    {
        public LotteryGameRepository(LotteryContext context)
        {
        }

        public IList<LotteryGame> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}