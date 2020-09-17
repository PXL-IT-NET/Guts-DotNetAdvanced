using System;
using Bank.Business.Interfaces;
using Bank.Data.DomainClasses;
using Bank.Data.Interfaces;

namespace Bank.Business
{
    public class AccountValidator : IAccountValidator
    {
        public AccountValidator(ICustomerRepository customerRepository)
        {
        }

        public ValidatorResult IsValid(Account account)
        {
            throw new NotImplementedException();
        }
    }
}