using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public class CaptainService : ICaptainService
    {
        private Captain _currentCaptain;
        public Captain SetupCaptain()
        {
            if(_currentCaptain == null) 
            {
                ConsoleWriter.Write("Greetings captain, what is your name?");
                var name = Console.ReadLine();
                var saves = SaveLoad.LookForSaves(name);
                if(saves.Count() > 0)
                {
                    _currentCaptain = SaveLoad.Load(saves.FirstOrDefault());
                    ConsoleWriter.Write($"Welcome back Captain {_currentCaptain.Name}");    
                    return _currentCaptain;
                }
                
                _currentCaptain = new Captain(name, new GameTime(DateTime.Now));
                _currentCaptain.Ship = new Ship("Bullrun","Sloop", 40000, _currentCaptain);
                _currentCaptain.Ship.CurrentLocation = Locations.Earth;
                _currentCaptain.Ship.Status = ShipStatus.InOrbit;
                ConsoleWriter.Write($"Welcome, Captain {name}, you have logged in on {_currentCaptain.StartTime.GetCurrentInGameTime()}");
                return _currentCaptain;
            }
            return _currentCaptain;
        }

        public string SetCourse(Location targetLocation)
        {
            var ship = _currentCaptain.Ship;

            _currentCaptain.Ship.Enqueue(new CourseSet(ship, ship.CurrentLocation, targetLocation, _currentCaptain.StartTime));
            //ship.ArrivalTime = TimeDilation.CalculateTime(_currentCaptain.GameLastSeen, _currentCaptain.RealLastSeen, DateTime.Now).Add(hoursToTargetLocation);
            return ""; //Return something to the UI Console
        }
    }
}
