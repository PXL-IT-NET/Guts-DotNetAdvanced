using System;

namespace Bank.Domain
{
    public class Account
    {
        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public AccountType AccountType { get; set; }

        public int CustomerId { get; set; }

        public static Account CreateNewForCustomer(int customerId, string accountNumber, AccountType type)
        {
            throw new NotImplementedException("The 'CreateNewForCustomer' method of 'Account' is not implemented correctly.");
        }
    }
}
