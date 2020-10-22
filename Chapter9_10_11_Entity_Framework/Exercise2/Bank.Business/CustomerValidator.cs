using System;
using Bank.Business.Contracts;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Business
{
    internal class CustomerValidator : ICustomerValidator
    {
        public CustomerValidator(ICityRepository cityRepository)
        {
        }

        public ValidatorResult IsValid(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
