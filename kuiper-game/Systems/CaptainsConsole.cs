using System;
using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Services;
using Kuiper.Systems.CommandInfrastructure;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICaptainService _captainService;
        private readonly ICommandParser _commandParser;
        private readonly Captain _currentCaptain;
        

        public CaptainsConsole(ICaptainService captainService, ICommandParser commandParser)
        {
            _captainService = captainService;
            _commandParser = commandParser;
            _currentCaptain = _captainService.SetupCaptain();
            
        }

        public void ConsoleMapper(string input)
        {
            _commandParser.ParseAndExecute(input);
        }
    }
}