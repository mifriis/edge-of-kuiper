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

        public Account(decimal balance, IList<Transaction> transactions)
        {
            _balance = balance;
            _transactions = transactions;
        }

        public decimal Balance { get { return _balance; } }

        public decimal Deposit(decimal amount) {
            _balance += amount;
            _transactions.Add(new Transaction(GameTime.Now(), TransactionType.Deposit, amount));
            return _balance;
        }

        public decimal Withdraw(decimal amount)
        {
            _balance -= amount;
            _transactions.Add(new Transaction(GameTime.Now(), TransactionType.Deposit, amount));
            return _balance;
        }

        public string DisplayTransActionHistory() {
            return _transactions.ToString();
        }
    }

    public class Transaction {

        public Transaction(DateTime timestampGametime, TransactionType action, decimal amount) {
            TimestampGametime = timestampGametime;
            Action = action;
            Amount = amount;
        }

        public DateTime TimestampGametime { get; }
        public TransactionType Action { get; }
        public decimal Amount { get; }
    }

    public enum TransactionType {
        Deposit,
        Withdrawal
    }
}