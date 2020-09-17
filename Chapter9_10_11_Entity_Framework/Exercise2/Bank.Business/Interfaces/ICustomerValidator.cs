using Bank.Data.DomainClasses;

namespace Bank.Business.Interfaces
{
    public interface ICustomerValidator
    {
        ValidatorResult IsValid(Customer customer);
    }
}