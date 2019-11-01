using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bank.Data.DomainClasses.Enums;

namespace Bank.Data.DomainClasses
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
