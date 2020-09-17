using System;
using Bank.Business.Interfaces;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.Business
{
    public class CustomerValidator : ICustomerValidator
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
