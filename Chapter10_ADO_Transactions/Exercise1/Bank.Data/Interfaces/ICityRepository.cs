using System.Collections.Generic;
using Bank.Data.DomainClasses;

namespace Bank.Data.Interfaces
{
    public interface ICityRepository
    {
        IList<City> GetAll();
    }
}