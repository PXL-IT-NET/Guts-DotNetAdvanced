using System.Collections.Generic;
using Bank.Domain;

namespace Bank.AppLogic.Contracts.DataAccess
{
    public interface ICityRepository
    {
        IReadOnlyList<City> GetAllOrderedByZipCode();
    }
}