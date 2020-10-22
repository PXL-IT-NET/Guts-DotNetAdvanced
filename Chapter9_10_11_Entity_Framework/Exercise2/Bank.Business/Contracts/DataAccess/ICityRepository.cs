using System.Collections.Generic;
using Bank.Domain;

namespace Bank.Business.Contracts.DataAccess
{
    public interface ICityRepository
    {
        IList<City> GetAll();
    }
}