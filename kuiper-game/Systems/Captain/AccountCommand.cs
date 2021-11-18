

using System.Linq;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class AccountCommand : CaptainBaseCommand
    {
        public AccountCommand(ICaptainService captainService, IGameTimeService gameTimeService) : base(captainService, gameTimeService)
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