using Bank.Domain;

namespace Bank.Business.Contracts
{
    public interface ICustomerValidator
    {
        ValidatorResult IsValid(Customer customer);
    }
}