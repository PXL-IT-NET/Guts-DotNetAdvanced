using System;
using System.Collections.Generic;

namespace Lottery.Domain
{
    public class Draw //=Trekking
    {
        public int Id { get; set; }

        public LotteryGame LotteryGame { get; set; }
        public int LotteryGameId { get; set; }

        public ICollection<DrawNumber> DrawNumbers { get; set; }

        public DateTime Date { get; set; }
    }
}
