

using System.Linq;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class AccountCommand : CaptainBaseCommand
    {
        public AccountCommand(ICaptainService captainService) : base(captainService)
        {
        }

        public override string Name => "account";

        public override void Execute(string[] args)
        {
            var captain = _captainService.GetCaptain();
            if(!captain.Account.Transactions.Any()) 
            {
                ConsoleWriter.Write("No transaction history available....");
                return;
            }

            ConsoleWriter.Write("Printing transaction history");
            ConsoleWriter.Write("--------------------------------------------------------------");
            ConsoleWriter.Write("Date                             |  Action      |  Amount");
            ConsoleWriter.Write("--------------------------------------------------------------");

            foreach (var transaction in captain.Account.Transactions)
            {
                ConsoleWriter.Write(transaction.ToString());
            }
            
            ConsoleWriter.Write("--------------------------------------------------------------");
        }
    }
} 

       /* private void DisplayAccountTransactionHistory() {
            if(!_currentCaptain.Account.Transactions.Any()) {
                ConsoleWriter.Write("No transaction history available....");
                return;
            }

            ConsoleWriter.Write("Printing transaction history");
            ConsoleWriter.Write("--------------------------------------------------------------");
            ConsoleWriter.Write("Date                             |  Action      |  Amount");
            ConsoleWriter.Write("--------------------------------------------------------------");

            foreach (var transaction in _currentCaptain.Account.Transactions)
            {
                ConsoleWriter.Write(transaction.ToString());
            }
            
            ConsoleWriter.Write("--------------------------------------------------------------");
        }
*/