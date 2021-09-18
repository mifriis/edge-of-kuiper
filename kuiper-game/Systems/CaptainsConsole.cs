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
        private readonly ISolarSystemService _solarSystemService;

        public CaptainsConsole(ICaptainService captainService, ISolarSystemService solarSystemService, ICommandParser commandParser)
        {
            _captainService = captainService;
            _solarSystemService = solarSystemService;
            _commandParser = commandParser;
            _currentCaptain = _captainService.SetupCaptain();
            
        }

        public void ConsoleMapper(string input)
        {
            _commandParser.ParseAndExecute(input);
        }
    }
}