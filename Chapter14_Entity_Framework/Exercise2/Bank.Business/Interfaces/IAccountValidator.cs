using Bank.Data.DomainClasses;

namespace Bank.Business.Interfaces
{
    public interface IAccountValidator
    {
        ValidatorResult IsValid(Account account);
    }
}