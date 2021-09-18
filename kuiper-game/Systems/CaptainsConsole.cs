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

        public CaptainsConsole(ICaptainService captainService, ISolarSystemService solarSystemService)
        {
            _captainService = captainService;
            _solarSystemService = solarSystemService;

            _currentCaptain = _captainService.SetupCaptain();
            
        }

        public void ConsoleMapper(string input)
        {
            switch (input)
            {
                case "help":
                    Help();
                    break;
                case "save":
                    Save();
                    break;
                case "time":
                    CurrentTime();
                    break;
                case "ship description":
                    Ship("description");
                    break;
                case "ship stats":
                    Ship("stats");
                    break;
                case "ship location":
                    Ship("location");
                    break;
                case "ship set course":
                    Ship("set course");
                    break;
                case "test solarsystem":
                    var earth = _solarSystemService.GetBody("Earth");
                    var mercury = _solarSystemService.GetBody("Mercury");

                    var dist = _solarSystemService.GetDistanceInAu(earth, mercury);
                    ConsoleWriter.Write($"Distance is {dist} AU.");
                    break;
                default:
                    ConsoleWriter.Write($"{input} not recognized. Try 'help' for list of commands");
                    break;
            }
        }
    }
}