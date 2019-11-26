using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.Data
{
    public class CityRepository : ICityRepository
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