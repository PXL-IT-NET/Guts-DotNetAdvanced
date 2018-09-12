using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.Data
{
    public class CityRepository : ICityRepository
    {
        public CityRepository(IConnectionFactory connectionFactory)
        {
        }

        public IList<City> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}