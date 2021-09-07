using System;
using System.Linq;
using Kuiper.Domain;

namespace Kuiper.Systems
{
    public class CaptainsConsole
    {
        Captain _currentCaptain;
        public CaptainsConsole()
        {
            if(_currentCaptain == null) 
            {
                SetupCaptain();
            }
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
                default:
                    ConsoleWriter.Write($"{Environment.NewLine}{input} not recognized. Try 'help' for list of commands");
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
                default:
                    ConsoleWriter.Write($"{Environment.NewLine}{subChoice} not recognized. Try ship location, ship stats or ship description");
                    break;
            }
        }

        public void Help()
        {
            ConsoleWriter.Write($"{Environment.NewLine}No help availiable.");

        }

        public void Save()
        {
            _currentCaptain.MarkLastSeen();
            SaveLoad.SaveGame(_currentCaptain);
            ConsoleWriter.Write($"{Environment.NewLine}Game saved successfully.", ConsoleColor.Red);
        }

        public void CurrentTime()
        {
            var currentGameTime = TimeDilation.CalculateTime(_currentCaptain.GameLastSeen, _currentCaptain.RealLastSeen, DateTime.Now);
            ConsoleWriter.Write($"{Environment.NewLine}The time is currently: {currentGameTime}");
        }

        public void SetupCaptain()
        {
            ConsoleWriter.Write("Greetings captain, what is your name?");
            var name = Console.ReadLine();
            var saves = SaveLoad.LookForSaves(name);
            if(saves.Count() > 0)
            {
                _currentCaptain = SaveLoad.Load(saves.FirstOrDefault());
                ConsoleWriter.Write($"{Environment.NewLine}Welcome back Captain {_currentCaptain.Name}, you were last seen on {_currentCaptain.GameLastSeen}!");    
                return;
            }
            _currentCaptain = new Captain(name, TimeDilation.GameStartDate, DateTime.Now);
            _currentCaptain.Ship = new Ship("Bullrun","Sloop", 40000);
            _currentCaptain.Ship.Location = Locations.Earth;
            ConsoleWriter.Write($"{Environment.NewLine}Welcome, Captain {name}, you have logged in on {_currentCaptain.GameLastSeen}");
        }
    }
}