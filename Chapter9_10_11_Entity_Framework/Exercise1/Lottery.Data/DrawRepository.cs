using System;
using System.Collections.Generic;
using System.Linq;
using Lottery.Business.Interfaces;
using Lottery.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Data
{
    public class DrawRepository : IDrawRepository
    {
        public DrawRepository(LotteryContext context)
        {
        }

        public IList<Draw> Find(int lotteryGameId, DateTime? fromDate, DateTime? untilDate)
        {
            return null;
        }

        public void Add(Draw draw)
        {
        }
    }
}