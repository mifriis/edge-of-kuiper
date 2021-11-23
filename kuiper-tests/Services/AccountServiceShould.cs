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
    public class AccountServiceShould
    {
        [Fact]
        public void DepositInAnAccount()
        {
            //Arrange
            var now = DateTime.Now;
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.Now()).Returns(now);
            var account = new Account(0);
            var accountService = new AccountService(gameTimeService.Object);
            accountService.Account = account;
            
            //Act
            var serviceBalance = accountService.Deposit(10);
            var balance = account.Balance;

            //Assert
            Assert.Equal(10, balance);
            Assert.Equal(10, serviceBalance);
            Assert.Equal(1, account.Transactions.Count());
        }

        [Fact]
        public void WithdrawFromAnAccount()
        {
            //Arrange
            var now = DateTime.Now;
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.Now()).Returns(now);
            var account = new Account(20);
            var accountService = new AccountService(gameTimeService.Object);
            accountService.Account = account;
            
            //Act
            var serviceBalance = accountService.Withdraw(10);
            var balance = account.Balance;

            //Assert
            Assert.Equal(10, balance);
            Assert.Equal(10, serviceBalance);
            Assert.Equal(1, account.Transactions.Count());
        }

        [Fact]
        public void ReturnTransactionalAcitivitesFromAnAccount()
        {
            //Arrange
            var now = DateTime.Now;
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.Now()).Returns(now);
            var account = new Account(0);
            var accountService = new AccountService(gameTimeService.Object);
            accountService.Account = account;

            account.Deposit(100, now);
            account.Withdraw(50, now);
            account.Deposit(10, now);

            //Act
            var transactions = accountService.Transactions().ToList();

            //Assert
            Assert.Equal(2, transactions.Count(u => u.Action == TransactionType.Deposit));
            Assert.Equal(1, transactions.Count(u => u.Action == TransactionType.Withdrawal));
        }
    }
}