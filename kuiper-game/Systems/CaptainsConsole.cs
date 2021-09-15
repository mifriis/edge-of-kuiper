using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICaptainService _captainService;
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
                case "account balance":
                    ConsoleWriter.Write($"Your current account balance is ${_currentCaptain.Account.Balance.ToString("N1")}");
                    break;
                case "account history":
                    _currentCaptain.Account.DisplayTransactionHistory();
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

        private void Ship(string subChoice)
        {
            switch (subChoice)
            {
                case "location":
                    ConsoleWriter.Write(_currentCaptain.Ship.LocationDescription);
                    break;
                case "stats":
                    ConsoleWriter.Write(_currentCaptain.Ship.ShipStatsDescription);
                    break;
                case "description":
                    ConsoleWriter.Write(_currentCaptain.Ship.Description);
                    break;
                case "set course":
                    SetCourse();
                    break;
                default:
                    ConsoleWriter.Write($"{subChoice} not recognized. Try ship location, ship stats or ship description");
                    break;
            }
        }

        private void SetCourse()
        {
            ConsoleWriter.Write($"What location should the ship set a course for?");
            foreach (var location in Locations.Destinations)
            {
                ConsoleWriter.Write($"* {location.Name}");
            }
            var input = Console.ReadLine();
            var target = Locations.Destinations.First(x => x.Name == input);
            if(target != null)
            {
                 var courseText = _captainService.SetCourse(target);
                 ConsoleWriter.Write(courseText);
                 return;
            }
            ConsoleWriter.Write($"No location found with the name {target}");
        }

        public void Help()
        {
            ConsoleWriter.Write($"No help availiable.");

        }

        public void Save()
        {
            _currentCaptain.LastLoggedIn = GameTime.Now();
            SaveLoad.SaveGame(_currentCaptain);
            ConsoleWriter.Write($"Game saved successfully.", ConsoleColor.Red);
        }

        public void CurrentTime()
        {
            var currentGameTime = GameTime.Now();
            ConsoleWriter.Write($"The time is currently: {currentGameTime}");
        }
    }
}