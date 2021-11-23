using System;
using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Systems;
using Kuiper.Systems.Events;

namespace Kuiper.Services
{
    public interface IAccountService
    {
        Account Account { get; set; }
        decimal Deposit(decimal amount);
        decimal Withdraw(decimal amount);
        IEnumerable<Transaction> Transactions();
    }
}