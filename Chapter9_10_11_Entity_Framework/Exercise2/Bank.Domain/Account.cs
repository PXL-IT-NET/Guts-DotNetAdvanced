using Bank.Domain.Enums;

namespace Bank.Domain
{
    public class Account
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public AccountType AccountType { get; set; }

        public int CustomerId { get; set; }
    }
}
