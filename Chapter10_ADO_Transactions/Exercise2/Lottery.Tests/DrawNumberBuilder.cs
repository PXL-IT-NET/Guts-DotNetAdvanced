using System;
using Lottery.Domain;

namespace Lottery.Tests
{
    public class DrawNumberBuilder
    {
        private readonly DrawNumber _drawNumber;
        private readonly Random _random;

        public DrawNumberBuilder(Random random)
        {
            _random = random ?? new Random();
            _drawNumber = new DrawNumber
            {
                Number = _random.Next(1, int.MaxValue),
                Position = null
            };
        }

        public DrawNumberBuilder WithPosition(int position)
        {
            _drawNumber.Position = position;
            return this;
        }

        public DrawNumber Build()
        {
            return _drawNumber;
        }
    }
}