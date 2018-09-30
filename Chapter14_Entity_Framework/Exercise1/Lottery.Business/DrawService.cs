using System;
using System.Collections.Generic;
using Lottery.Business.Interfaces;
using Lottery.Data.Interfaces;
using Lottery.Domain;

namespace Lottery.Business
{
    public class DrawService : IDrawService
    {
        public DrawService(IDrawRepository drawRepository)
        {
        }

        public void CreateDrawFor(LotteryGame lotteryGame)
        {
        }
    }
}
