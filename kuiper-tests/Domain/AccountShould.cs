using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;
using Kuiper.Domain;
using Kuiper.Systems.Events;
using Lamar;

namespace Kuiper.Tests.Unit.Services
{
    public class AccountShould
    {
        [Fact]
        public void DepositSucessfully()
        {
            //Arrange
            var account = new Account(0);
            var now = DateTime.Now;

            //Act
            account.Deposit(10, now);
            var balance = account.Balance;

            //Assert
            Assert.Equal(10, balance);
            Assert.Equal(1, account.Transactions.Count());
        }

        [Fact]
        public void WithdrawSucessfully()
        {
            //Arrange
            var account = new Account(20);
            var now = DateTime.Now;

            //Act
            account.Withdraw(10, now);
            var balance = account.Balance;

            //Assert
            Assert.Equal(10, balance);
            Assert.Equal(1, account.Transactions.Count());
        }

        [Fact]
        public void KeepTransactionOfAcitvities()
        {
            //Arrange
            var account = new Account(0);
            var now = DateTime.Now;

            account.Deposit(100, now);
            account.Withdraw(50, now);
            account.Deposit(10, now);

            //Act
            var transactions = account.Transactions;

            //Assert
            Assert.Equal(2, transactions.Count(u => u.Action == TransactionType.Deposit));
            Assert.Equal(1, transactions.Count(u => u.Action == TransactionType.Withdrawal));
        }

        [Fact]
        public void TransactionsHaveHumanOutput()
        {
            //Arrange
            var account = new Account(0);
            var now = DateTime.Now;

            account.Deposit(100, now);
            account.Withdraw(50, now);
            account.Deposit(10, now);

            //Act
            var transactionString = account.Transactions.FirstOrDefault().ToString();

            //Assert
            Assert.Contains("$100.0", transactionString);
            Assert.Contains("Deposit", transactionString);
            
        }
    }
}