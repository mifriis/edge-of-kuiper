using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly Captain _currentCaptain;
        private readonly ISolarSystemService _solarSystemService;

        public CaptainsConsole(ISolarSystemService solarSystemService)
        {
            _solarSystemService = solarSystemService;

            _solarSystemService.SetupGame();
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
                    ConsoleWriter.Write(SolarSystemLocator.SolarSystem.Captain.Ship.ScanForAsteroids());
                    break;
                case "mine":
                    Mine();
                    break;
                case "test solarsystem":
                    var earth = _solarSystemService.GetBody("Earth");
                    var mercury = _solarSystemService.GetBody("Mercury");

                    var dist = _solarSystemService.GetDistanceInAu(earth, mercury);
                    ConsoleWriter.Write($"Distance is {dist} AU.");
                    break;
                default:
                    ConsoleWriter.Write($"{subChoice} not recognized. Try ship location, ship stats or ship description");
                    break;
            }
        }

        private void Mine()
        {
            ConsoleWriter.Write($"How many hours do you wish to mine for?");
            int hours;
            var input = int.TryParse(Console.ReadLine(), out hours);
            if(input) 
            {
                ConsoleWriter.Write(SolarSystemLocator.SolarSystem.Captain.Ship.MineAsteroid(hours));
            }
            else
            {
                ConsoleWriter.Write($"Must be a numerical value like '5'");
                Mine();
            }
        }

        private void Ship(string subChoice)
        {
            switch (subChoice)
            {
                case "location":
                    ConsoleWriter.Write(SolarSystemLocator.SolarSystem.Captain.Ship.LocationDescription);
                    break;
                case "stats":
                    ConsoleWriter.Write(SolarSystemLocator.SolarSystem.Captain.Ship.ShipStatsDescription);
                    break;
                case "description":
                    ConsoleWriter.Write(SolarSystemLocator.SolarSystem.Captain.Ship.Description);
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
            foreach (var location in Locations.Destinations) //A change for github
            {
                ConsoleWriter.Write($"* {location.Name}");
            }
            var input = Console.ReadLine();
            var target = Locations.Destinations.First(x => x.Name == input);
            if(target != null)
            {
                 var courseText = SolarSystemLocator.SolarSystem.Captain.Ship.SetCourse(target);
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
            SolarSystemLocator.SolarSystem.Captain.LastLoggedIn = GameTime.Now();
            SaveLoad.SaveGame(SolarSystemLocator.SolarSystem);
            ConsoleWriter.Write($"Game saved successfully.", ConsoleColor.Red);
        }

        public void CurrentTime()
        {
            var currentGameTime = GameTime.Now();
            ConsoleWriter.Write($"The time is currently: {currentGameTime}");
        }
    }
}