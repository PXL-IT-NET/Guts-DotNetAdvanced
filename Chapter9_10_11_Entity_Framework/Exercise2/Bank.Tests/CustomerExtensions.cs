using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bank.Domain;
using NUnit.Framework;

namespace Bank.Tests
{
    internal static class CustomerExtensions
    {
        public static ICollection<Account> TryGetAccounts(this Customer customer)
        {
            var accountsProperty = GetAccountsProperty();

            var value = accountsProperty.GetValue(customer);
            if (value == null) return null;

            var accounts = value as ICollection<Account>;
            if (accounts == null)
            {
                Assert.Fail("The Accounts property of a customer should be assignable to an ICollection<Account>.");
            }

            return accounts;
        }

        public static void TrySetAccounts(this Customer customer, ICollection<Account> accounts)
        {
            var accountsProperty = GetAccountsProperty();
            accountsProperty.SetValue(customer,accounts);
        }

        private static PropertyInfo GetAccountsProperty()
        {
            var type = typeof(Customer);
            var accountsProperty = type.GetProperties()
                .FirstOrDefault(p => typeof(IEnumerable<Account>).IsAssignableFrom(p.PropertyType));

            if (accountsProperty == null)
            {
                Assert.Fail("Cannot find a property in the Customer class that holds a collection of Accounts.");
            }

            return accountsProperty;
        }
    }
}