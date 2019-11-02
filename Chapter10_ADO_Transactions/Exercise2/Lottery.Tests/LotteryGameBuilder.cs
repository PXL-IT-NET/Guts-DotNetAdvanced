using System;
using System.Collections.Generic;
using Lottery.Domain;

namespace Lottery.Tests
{
    public class LotteryGameBuilder
    {
        private readonly LotteryGame _game;
        private readonly Random _random;

        public LotteryGameBuilder()
        {
            _random = new Random();
            _game = new LotteryGame
            {
                Name = Guid.NewGuid().ToString(),
                MaximumNumber = _random.Next(1, 100)
            };
            _game.NumberOfNumbersInADraw = _random.Next(1, _game.MaximumNumber + 1);
        }

        public LotteryGameBuilder WithId()
        {
            _game.Id = _random.Next(1, int.MaxValue);
            return this;
        }

        public LotteryGameBuilder WithMaximumNumber(int maximumNumber)
        {
            _game.MaximumNumber = maximumNumber;
            return this;
        }

        public LotteryGameBuilder WithMaximumNumberOfNumbersInADraw(int numberOfNumbersInADraw)
        {
            _game.NumberOfNumbersInADraw = numberOfNumbersInADraw;
            return this;
        }

        //public LotteryGameBuilder WithRandomDraws(int minimumAmountOfDraws, int maximumAmountOfDraws)
        //{
        //    _game.Draws = new List<Draw>();
        //    var amountOfDraws = _random.Next(minimumAmountOfDraws, maximumAmountOfDraws + 1);
        //    for (int i = 0; i < amountOfDraws; i++)
        //    {
        //        var draw = new DrawBuilder().WithRandomDrawNumbers(1,10).Build();
        //        _game.Draws.Add(draw);
        //    }
        //    return this;
        //}

        //public LotteryGameBuilder WithDrawsAroundDateRange(DateTime? from, DateTime? until)
        //{
        //    _game.Draws = new List<Draw>();

        //    if (from.HasValue)
        //    {
        //        _game.Draws.Add(new DrawBuilder().WithDate(from.Value.AddMinutes(-1)).Build());
        //        _game.Draws.Add(new DrawBuilder().WithDate(from.Value).Build());
        //        _game.Draws.Add(new DrawBuilder().WithDate(from.Value.AddMinutes(1)).Build());
        //    }

        //    if (until.HasValue)
        //    {
        //        _game.Draws.Add(new DrawBuilder().WithDate(until.Value.AddMinutes(-1)).Build());
        //        _game.Draws.Add(new DrawBuilder().WithDate(until.Value).Build());
        //        _game.Draws.Add(new DrawBuilder().WithDate(until.Value.AddMinutes(1)).Build());
        //    }

        //    return this;
        //}

        public LotteryGame Build()
        {
            return _game;
        }
    }
}