using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kuiper.Domain
{
    public class Account
    {
        private decimal _balance;
        private IList<Transaction> _transactions;

        [JsonConstructor]
        public Account(decimal balance)
        {
            _balance = balance;
            _transactions = new List<Transaction>();
        }

        public decimal Balance {
            get {
                return _balance;
            }
        }

        public IEnumerable<Transaction> Transactions {
            get {
                return _transactions;
            }
        }

        public decimal Deposit(decimal amount, DateTime depositTime) {
            _balance += amount;
            _transactions.Add(new Transaction(depositTime, TransactionType.Deposit, amount));
            return _balance;
        }

        public decimal Withdraw(decimal amount, DateTime withdrawTime)
        {
            _balance -= amount;
            _transactions.Add(new Transaction(withdrawTime, TransactionType.Withdrawal, amount));
            return _balance;
        }
    }
}