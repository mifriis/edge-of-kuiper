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
            this.shipService.SetShip(_captainService.GetCaptain().Ship);
        }
       
        public void ConsoleMapper(string input)
        {
            _commandParser.ParseAndExecute(input);
        }
    }
}