using System;
using System.Collections.Generic;
using Lottery.Domain;

namespace Lottery.AppLogic.Interfaces
{
    public interface IDrawRepository
    {
        IList<Draw> Find(int lotteryGameId, DateTime? fromDate, DateTime? untilDate);
        void Add(Draw draw);
    }
}