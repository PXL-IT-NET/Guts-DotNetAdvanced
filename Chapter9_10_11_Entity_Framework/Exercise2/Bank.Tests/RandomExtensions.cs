using System;
using Bank.Domain;

namespace Bank.Tests
{
    internal static class RandomExtensions
    {
        public static AccountType NextAccountType(this Random random)
        {
            var accountTypeValues = Enum.GetValues(typeof(AccountType));
            return (AccountType)accountTypeValues.GetValue(random.Next(accountTypeValues.Length));
        }
    }
}