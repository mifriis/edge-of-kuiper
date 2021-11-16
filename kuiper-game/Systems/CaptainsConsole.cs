using System;
using System.Linq;
using Kuiper.Services;
using Kuiper.Systems.CommandInfrastructure;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICommandParser _commandParser;
        private readonly ICaptainService _captainService;
        private readonly IShipService shipService;

        public CaptainsConsole(ICommandParser commandParser, ICaptainService captainService, IShipService shipService)
        {

            _commandParser = commandParser;
            _captainService = captainService;
            this.shipService = shipService;
            _captainService.SetupCaptain();
            this.shipService.Ship = _captainService.GetCaptain().Ship;
        }
       
        public void ConsoleMapper(string input)
        {
            _commandParser.ParseAndExecute(input);
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
        
