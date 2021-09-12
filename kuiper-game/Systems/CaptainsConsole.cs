using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICaptainService _captainService;

        public CaptainsConsole(ICaptainService captainService)
        {
            _captainService = captainService;

            _captainService.SetupCaptain();
        }
        public void ConsoleMapper(string input)
        {
            if(input.StartsWith("ship"))
            {
                    Ship(input.Substring(5));
                    return;
            }

            if(input.StartsWith("mining"))
            {
                    Mining(input.Substring(7));
                    return;
            }

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
                default:
                    ConsoleWriter.Write($"{input} not recognized. Try 'help' for list of commands");
                    break;
            }
        }

        private void Mining(string subChoice)
        {
             switch (subChoice)
            {
                case "scan":
                    ConsoleWriter.Write(CaptainLocator.Captain.Ship.ScanForAsteroids());
                    break;
                default:
                    ConsoleWriter.Write($"{subChoice} not recognized. Try ship location, ship stats or ship description");
                    break;
            }
        }

        private void Ship(string subChoice)
        {
            switch (subChoice)
            {
                case "location":
                    ConsoleWriter.Write(CaptainLocator.Captain.Ship.LocationDescription);
                    break;
                case "stats":
                    ConsoleWriter.Write(CaptainLocator.Captain.Ship.ShipStatsDescription);
                    break;
                case "description":
                    ConsoleWriter.Write(CaptainLocator.Captain.Ship.Description);
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
            CaptainLocator.Captain.LastLoggedIn = GameTime.Now();
            SaveLoad.SaveGame(CaptainLocator.Captain);
            ConsoleWriter.Write($"Game saved successfully.", ConsoleColor.Red);
        }

        public void CurrentTime()
        {
            var currentGameTime = GameTime.Now();
            ConsoleWriter.Write($"The time is currently: {currentGameTime}");
        }
    }
}