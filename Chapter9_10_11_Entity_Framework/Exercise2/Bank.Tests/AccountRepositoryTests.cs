using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Data;
using Bank.Data.DomainClasses;
using Bank.Data.DomainClasses.Enums;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("dotnet2", "H12", "Exercise02",
         @"Bank.Data\DomainClasses\Account.cs;
Bank.Data\DomainClasses\Customer.cs;
Bank.Data\BankContext.cs;
Bank.Data\AccountRepository.cs;
Bank.Data\CityRepository.cs;
Bank.Data\CustomerRepository.cs;
Bank.Business\AccountValidator.cs;
Bank.Business\CustomerValidator.cs;
Bank.UI\AccountsWindow.xaml;
Bank.UI\AccountsWindow.xaml.cs;
Bank.UI\CustomersWindow.xaml;
Bank.UI\CustomersWindow.xaml.cs;
Bank.UI\TransferWindow.xaml;
Bank.UI\TransferWindow.xaml.cs")]
    internal class AccountRepositoryTests : DatabaseTests
    {
        
        [MonitoredTest("AccountRepository - Add should add a new account to the database")]
        public void Add_ShouldAddANewAccountToTheDatabase()
        {
            //Arrange
            Customer existingCustomer;
            using (var context = CreateDbContext())
            {
                City existingCity = CreateExistingCity(context);
                existingCustomer = new CustomerBuilder().WithZipCode(existingCity.ZipCode).Build();
                context.Set<Customer>().Add(existingCustomer);
                context.SaveChanges();
            }

            var newAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act
                repo.Add(newAccount);

                //Assert
                var addedAccount = context.Set<Account>().FirstOrDefault(a => a.CustomerId == existingCustomer.Id);
                Assert.That(addedAccount, Is.Not.Null,
                    "The account is not added correctly to the database.");
                Assert.That(addedAccount.AccountNumber, Is.EqualTo(newAccount.AccountNumber),
                    "The 'AccountNumber' is not saved correctly.");
                Assert.That(addedAccount.AccountType, Is.EqualTo(newAccount.AccountType),
                    "The 'AccountType' is not saved correctly.");
                Assert.That(addedAccount.Balance, Is.EqualTo(newAccount.Balance),
                    "The 'Balance' is not saved correctly.");
            }
        }

        [MonitoredTest("AccountRepository - Add should not save customer relation in a disconnected scenario")]
        public void Add_ShouldNotSaveCustomerRelationInADisconnectedScenario()
        {
            //Arrange
            Customer existingCustomer;
            using (var context = CreateDbContext())
            {
                existingCustomer = CreateExistingCustomer(context);
            }

            var newAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();
            existingCustomer.TrySetCity(null);
            existingCustomer.TrySetAccounts(null);
            newAccount.TrySetCustomer(existingCustomer);

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act
                try
                {
                    repo.Add(newAccount);
                }
                catch (DbUpdateException updateException)
                {
                    if (updateException.InnerException != null && updateException.InnerException.Message.ToLower()
                            .Contains("unique constraint"))
                    {
                        Assert.Fail(
                            "If the 'Customer' navigation property is set to an untracked instance of customer, " +
                            "the application tries to add the customer to the database. " +
                            "Make sure relations are not saved.");
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    Assert.Fail("Something went wrong when adding the Account.");
                }

            }
        }

        [MonitoredTest("AccountRepository - Add should throw an ArgumentException when the account already exists in the database")]
        public void Add_ShouldThrowArgumentExceptionWhenTheAccountAlreadyExists()
        {
            //Arrange
            Account existingAccount;
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                existingAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();
                context.Set<Account>().Add(existingAccount);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act + Assert
                Assert.That(() => repo.Add(existingAccount), Throws.ArgumentException);
            }
        }

        [MonitoredTest("AccountRepository - Update should update an existing account in the database")]
        public void Update_ShouldUpdateAnExistingAccountInTheDatabase()
        {
            //Arrange
            Account existingAccount;
            IList<Account> allOriginalAccounts;
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                existingAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();
                context.Set<Account>().Add(existingAccount);
                context.SaveChanges();
                allOriginalAccounts = context.Set<Account>().ToList();
            }

            var existingAccountId = existingAccount.Id;
            var newAccountNumber = Guid.NewGuid().ToString();
            var newAccountType = AccountType.PremiumAccount;

            using (var context = CreateDbContext())
            {
                existingAccount = context.Set<Account>().Find(existingAccountId);
                existingAccount.AccountNumber = newAccountNumber;
                existingAccount.AccountType = newAccountType;
             
                var repo = new AccountRepository(context);

                //Act
                repo.Update(existingAccount);
            }

            using (var context = CreateDbContext())
            {
                var updatedAccount = context.Set<Account>().Find(existingAccountId);

                //Assert
                var allAccounts = context.Set<Account>().ToList();
                Assert.That(allAccounts, Has.Count.EqualTo(allOriginalAccounts.Count),
                    "The amount of accounts in the database changed.");

                Assert.That(updatedAccount.AccountNumber, Is.EqualTo(newAccountNumber), "Accountnumber is not updated properly.");
                Assert.That(updatedAccount.AccountType, Is.EqualTo(newAccountType), "Account type is not updated properly.");
            }
        }

        [MonitoredTest("AccountRepository - Update should throw an ArgumentException when the account does not exist in the database")]
        public void Update_ShouldThrowArgumentExceptionWhenTheAccountDoesNotExists()
        {
            //Arrange
            var newAccount = new AccountBuilder().WithId(0).Build();

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act + Assert
                Assert.That(() => repo.Update(newAccount), Throws.ArgumentException);
            }
        }

        [MonitoredTest("AccountRepository - Update should throw an InvalidOperationException when the balance is updated")]
        public void Update_ShouldThrowAnInvalidOperationExceptionWhenTheBalanceIsUpdated()
        {
            //Arrange
            Account existingAccount;
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                existingAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();
                context.Set<Account>().Add(existingAccount);
                context.SaveChanges();
            }
            var existingAccountId = existingAccount.Id;

            using (var context = CreateDbContext())
            {
                existingAccount = context.Set<Account>().Find(existingAccountId);
                existingAccount.Balance += 1;

                var repo = new AccountRepository(context);

                //Act + Assert
                Assert.That(() => repo.Update(existingAccount),
                    Throws.InvalidOperationException,
                    "No InvalidOperationException thrown. " +
                    "Tip: an instance of the Entry class has a Property method that can be used to get the original and current value of a property of an entity. " +
                    "So you can compare the original value and current value of the Balance property.");
            }
        }

        [MonitoredTest("AccountRepository - TransferMoney should get the 2 accounts from te database, change the balances and save the changes")]
        public void TransferMoney_ShouldGetThe2AccountsChangeTheBalancesAndSaveTheChanges()
        {
            Account fromAccount;
            Account toAccount;
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                fromAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id)
                    .WithBalance(RandomGenerator.Next(500, 1001)).Build();
                toAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();
                context.Set<Account>().Add(fromAccount);
                context.Set<Account>().Add(toAccount);
                context.SaveChanges();
            }
            var fromAccountId = fromAccount.Id;
            var toAccountId = toAccount.Id;
            decimal transferAmount = RandomGenerator.Next(100, 401);
            var expectedFromBalance = fromAccount.Balance - transferAmount;
            var expectedToBalance = toAccount.Balance + transferAmount;

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act
                repo.TransferMoney(fromAccountId, toAccountId, transferAmount);
            }

            using (var context = CreateDbContext())
            {
                var updatedFromAccount = context.Set<Account>().Find(fromAccountId);
                var updatedToAccount = context.Set<Account>().Find(toAccountId);

                //Assert
                Assert.That(updatedFromAccount.Balance, Is.EqualTo(expectedFromBalance), "Balance of the 'from account' is not updated correctly.");
                Assert.That(updatedToAccount.Balance, Is.EqualTo(expectedToBalance), "Balance of the 'to account' is not updated correctly.");
            }
        }
    }
}
