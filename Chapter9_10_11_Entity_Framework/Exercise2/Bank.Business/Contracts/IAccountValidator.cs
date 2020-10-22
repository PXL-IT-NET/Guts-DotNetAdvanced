using Bank.Domain;

namespace Bank.Business.Contracts
{
    public interface IAccountValidator
    {
        ValidatorResult IsValid(Account account);
    }
}