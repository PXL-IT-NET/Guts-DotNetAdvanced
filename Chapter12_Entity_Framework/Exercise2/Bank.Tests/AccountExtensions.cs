using System.Linq;
using System.Reflection;
using Bank.Data.DomainClasses;
using NUnit.Framework;

namespace Bank.Tests
{
    internal static class AccountExtensions
    {
        public static Customer TryGetCustomer(this Account account)
        {
            var customerProperty = GetCustomerProperty();

            var value = customerProperty.GetValue(account);
            if (value == null) return null;

            var customer = value as Customer;
            if (customer == null)
            {
                Assert.Fail("The customer property of an Account should be of type Customer.");
            }

            return customer;
        }

        public static void TrySetCustomer(this Account account, Customer customer)
        {
            var customerProperty = GetCustomerProperty();
            customerProperty.SetValue(account, customer);
        }

        private static PropertyInfo GetCustomerProperty()
        {
            var type = typeof(Account);
            var customerProperty = type.GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(Customer));

            if (customerProperty == null)
            {
                Assert.Fail("Cannot find a property in the Account class that holds a Customer.");
            }

            return customerProperty;
        }
    }
}