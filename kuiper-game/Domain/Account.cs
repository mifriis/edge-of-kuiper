using System;
using System.Collections.Generic;
using Kuiper.Systems;
using Newtonsoft.Json;

namespace Kuiper.Domain
{
    public class Account
    {
        private decimal _balance;
        [JsonProperty("transactions")]
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
            _transactions.Add(new Transaction(GameTime.Now(), TransactionType.Withdrawal, amount));
            return _balance;
        }

        public void DisplayTransactionHistory() {
            if(_transactions.Count == 0) {
                ConsoleWriter.Write("No transaction history available....");
                return;
            }

            ConsoleWriter.Write("Printing transaction history");
            ConsoleWriter.Write("--------------------------------------------------------------");
            ConsoleWriter.Write("Date                             |  Action      |  Amount");
            ConsoleWriter.Write("--------------------------------------------------------------");

            foreach (var transaction in _transactions)
            {
                ConsoleWriter.Write(transaction.ToString());
            }
            
            ConsoleWriter.Write("--------------------------------------------------------------");
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

        public override string ToString()
        {
            var format = @"{0:dddd, dd MMMM yyyy HH:mm:ss}  |  {1,-10}  |  ${2,-10:N1}";
            return String.Format(format, TimestampGametime, Action, Amount);
        }
    }

    public enum TransactionType {
        Deposit,
        Withdrawal
    }
}