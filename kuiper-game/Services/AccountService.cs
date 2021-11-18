using System;
using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Systems;
using Kuiper.Systems.Events;
using Lamar;

namespace Kuiper.Services
{
    public class AccountService : IAccountService
    {
        private IGameTimeService _gameTimeService;

        public Account Account { get; set; }

        public AccountService(IGameTimeService gameTimeService)
        {
            _gameTimeService = gameTimeService;
        }
        public decimal Deposit(decimal amount) {
            return Account.Deposit(amount, _gameTimeService.Now());
        }

        public decimal Withdraw(decimal amount)
        {
            return Account.Withdraw(amount, _gameTimeService.Now());
        }

        public IEnumerable<Transaction> Transactions()
        {
            return Account.Transactions;
        }
    }
}