using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bank.Data.DomainClasses;
using NUnit.Framework;

namespace Bank.Tests
{
    internal static class CustomerExtensions
    {
        public static IEnumerable<Account> TryGetAccounts(this Customer customer)
        {
            var accountsProperty = GetAccountsProperty();

            var value = accountsProperty.GetValue(customer);
            if (value == null) return null;

            var accounts = value as IEnumerable<Account>;
            if (accounts == null)
            {
                Assert.Fail("The Accounts property of a customer should be assignable to an IEnumerable<Account>.");
            }

            return accounts;
        }

        public static void TrySetAccounts(this Customer customer, ICollection<Account> accounts)
        {
            var accountsProperty = GetAccountsProperty();
            accountsProperty.SetValue(customer,accounts);
        }

        public static void TrySetCity(this Customer customer, City city)
        {
            var cityProperty = GetCityProperty();
            cityProperty.SetValue(customer, city);
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

        private static PropertyInfo GetCityProperty()
        {
            var type = typeof(Customer);
            var cityProperty = type.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(City));

            if (cityProperty == null)
            {
                Assert.Fail("Cannot find a property in the Customer class that holds a City.");
            }

            return cityProperty;
        }
    }
}