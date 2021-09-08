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

        public CaptainsConsole(ICaptainService captainService)
        {
            _captainService = captainService;

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
            _currentCaptain.MarkLastSeen();
            SaveLoad.SaveGame(_currentCaptain);
            ConsoleWriter.Write($"Game saved successfully.", ConsoleColor.Red);
        }

        public void CurrentTime()
        {
            var currentGameTime = TimeDilation.CalculateTime(_currentCaptain.GameLastSeen, _currentCaptain.RealLastSeen, DateTime.Now);
            ConsoleWriter.Write($"The time is currently: {currentGameTime}");
        }
    }
}