using System;
using System.Collections.Generic;
using System.Linq;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Infrastructure
{
    internal class CityRepository : ICityRepository
    {
        public CityRepository(BankContext context)
        {
        }

        public IReadOnlyList<City> GetAllOrderedByZipCode()
        {
            throw new NotImplementedException();
        }
    }
}