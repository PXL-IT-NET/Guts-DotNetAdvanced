using System.Collections.Generic;

namespace Lottery.Domain
{
    public class LotteryGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfNumbersInADraw { get; set; }
        public int MaximumNumber { get; set; }
    }
}