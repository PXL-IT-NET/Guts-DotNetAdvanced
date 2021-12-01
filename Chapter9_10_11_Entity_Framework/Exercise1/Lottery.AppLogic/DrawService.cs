using System;
using System.Collections.Generic;
using Lottery.AppLogic.Interfaces;
using Lottery.Domain;

namespace Lottery.AppLogic
{
    internal class DrawService : IDrawService
    {
        public DrawService(IDrawRepository drawRepository)
        {
        }

        public void CreateDrawFor(LotteryGame lotteryGame)
        {
        }
    }
}
