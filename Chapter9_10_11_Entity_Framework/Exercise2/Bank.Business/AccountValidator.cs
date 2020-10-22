using System;
using Bank.Business.Contracts;
using Bank.Business.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Business
{
    internal class AccountValidator : IAccountValidator
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