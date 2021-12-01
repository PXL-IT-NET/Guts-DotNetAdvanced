using System;
using Bank.Domain;

namespace Bank.Tests
{
    internal class CityBuilder
    {
        private readonly City _city;
        private static readonly Random Random = new Random();

        public CityBuilder()
        {
            _city = new City
            {
                Name = Guid.NewGuid().ToString(),
                ZipCode = Random.Next(1000, 10000)
            };
        }

        public City Build()
        {
            return _city;
        }
    }
}