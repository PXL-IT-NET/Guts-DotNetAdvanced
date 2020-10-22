using System;
using System.Collections.Generic;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Data
{
    internal class CityRepository : ICityRepository
    {
        public CityRepository(BankContext context)
        {
        }

        public IList<City> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}