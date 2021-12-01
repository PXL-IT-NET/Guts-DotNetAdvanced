using System;
using System.Collections.Generic;
using Lottery.AppLogic.Interfaces;
using Lottery.Domain;

namespace Lottery.Infrastructure
{
    internal class DrawRepository : IDrawRepository
    {
        public DrawRepository(LotteryContext context)
        {
        }

        public IList<Draw> Find(int lotteryGameId, DateTime? fromDate, DateTime? untilDate)
        {
            throw new NotImplementedException();
        }

        public void Add(Draw draw)
        {
        }
    }
}