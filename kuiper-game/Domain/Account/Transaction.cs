using System;

namespace Kuiper.Domain
{
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
}