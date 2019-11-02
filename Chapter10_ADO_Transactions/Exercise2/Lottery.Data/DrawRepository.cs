using System;
using System.Collections.Generic;
using Lottery.Data.Interfaces;
using Lottery.Domain;

namespace Lottery.Data
{
    public class DrawRepository : IDrawRepository
    {
        public DrawRepository(IConnectionFactory connectionFactory)
        {
        }

        public IList<Draw> Find(int lotteryGameId, DateTime? fromDate, DateTime? untilDate)
        {
            return null;
        }

        public void Add(int lotteryGameId, IList<int> numbers)
        {
        }
    }
}