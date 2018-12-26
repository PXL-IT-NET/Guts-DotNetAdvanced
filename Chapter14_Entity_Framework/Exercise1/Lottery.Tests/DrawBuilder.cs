using System;
using System.Collections.Generic;
using Lottery.Domain;

namespace Lottery.Tests
{
    public class DrawBuilder
    {
        private readonly Draw _draw;
        private readonly Random _random;

        public DrawBuilder()
        {
            _random = new Random();
            _draw = new Draw
            {
                Date = DateTime.Now.AddDays(-1 * _random.Next(100)),
            };
        }

        public DrawBuilder WithLotteryGameId(int gameId)
        {
            _draw.LotteryGameId = gameId;
            return this;
        }

        public DrawBuilder WithDate(DateTime date)
        {
            _draw.Date = date;
            return this;
        }

        public DrawBuilder WithRandomDrawNumbers(int minimumAmount, int maximumAmount)
        {
            _draw.DrawNumbers = new List<DrawNumber>();
            var amountOfNumbers = _random.Next(minimumAmount, maximumAmount + 1);
            for (int i = 0; i < amountOfNumbers; i++)
            {
                var drawNumber = new DrawNumberBuilder(_random).WithNumber(i + 1).Build();
                _draw.DrawNumbers.Add(drawNumber);
            }
            return this;
        }

        public Draw Build()
        {
            return _draw;
        }
    }
}