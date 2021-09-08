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
                    ConsoleWriter.Write($"Welcome back Captain {_currentCaptain.Name}, you were last seen on {_currentCaptain.GameLastSeen}!");    
                    return _currentCaptain;
                }
                
                _currentCaptain = new Captain(name, TimeDilation.GameStartDate, DateTime.Now);
                _currentCaptain.Ship = new Ship("Bullrun","Sloop", 40000);
                _currentCaptain.Ship.CurrentLocation = Locations.Earth;
                _currentCaptain.Ship.Status = ShipStatus.InOrbit;
                ConsoleWriter.Write($"Welcome, Captain {name}, you have logged in on {_currentCaptain.GameLastSeen}");
                return _currentCaptain;
            }
            return _currentCaptain;
        }

        public string SetCourse(Location targetLocation)
        {
            var ship = _currentCaptain.Ship;
            if(targetLocation == ship.CurrentLocation)
            {
                return $"{ship.Name} is already in orbit above {ship.CurrentLocation.Name}";
            }
            if(targetLocation == ship.TargetLocation)
            {
                return $"{ship.Name} is already enroute to {ship.TargetLocation.Name}";
            }
            ship.Status = ShipStatus.Enroute;
            ship.TargetLocation = targetLocation;
            long distance = 0;
            if(targetLocation.Sattelites.Contains(ship.CurrentLocation))
            {
                //Travel from a moon to parent
                distance = ship.CurrentLocation.OrbitalRadius;
                
            }
            if(ship.CurrentLocation.Sattelites.Contains(targetLocation))
            {
                //Travel from a parent to one of it's moons
                distance = targetLocation.OrbitalRadius;
            }
            if(distance == 0)
            {
                throw new NotImplementedException("Don't go to Mars just yet");
            }
            var hoursToTargetLocation = TimeSpan.FromHours(distance/ship.Speed);
            ship.ArrivalTime = TimeDilation.CalculateTime(_currentCaptain.GameLastSeen, _currentCaptain.RealLastSeen, DateTime.Now).Add(hoursToTargetLocation);
            return $"{ship.Name} will arrive in orbit above {targetLocation.Name} on {ship.ArrivalTime}";
        }
    }
}
