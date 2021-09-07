using System;
using System.Linq;
using Kuiper.Systems;

namespace Kuiper.Domain
{
    public class Ship
    {
        public Ship(string name, string shipClass, int speed, Captain captain)
        {
            Name = name;
            ShipClass = shipClass;
            Speed = speed;
            Captain = captain;
        }

        public string Name { get; }

        public string ShipClass { get; }
        public int Speed { get; }
        public Captain Captain { get; }
        public Location CurrentLocation { get; set; }
        public Location TargetLocation { get; set; }
        public DateTime ArrivalTime { get; set;}

        public ShipStatus Status { get; set; }

        public string Description { 
            get {
                return $"{Environment.NewLine}{Name}, a {ShipClass} in orbit above {CurrentLocation.Name}";
            }
        }

        public string ShipStatsDescription { 
            get {
                return $"{Environment.NewLine}Name: {Name}{Environment.NewLine}Class: {ShipClass}{Environment.NewLine}Speed: {Speed} km/h";
            }
        }

        public string LocationDescription { 
            get {
                return $"{Environment.NewLine}{Name} is in orbit above {CurrentLocation.Name}";
            }
        }

        public string SetCourse(Location targetLocation)
        {
            if(targetLocation == CurrentLocation)
            {
                return $"{Name} is already in orbit above {CurrentLocation.Name}";
            }
            Status = ShipStatus.Enroute;
            TargetLocation = targetLocation;
            long distance = 0;
            if(targetLocation.Sattelites.Contains(CurrentLocation))
            {
                //Travel from a moon to parent
                distance = CurrentLocation.OrbitalRadius;
                
            }
            if(CurrentLocation.Sattelites.Contains(targetLocation))
            {
                //Travel from a parent to one of it's moons
                distance = targetLocation.OrbitalRadius;
            }
            if(distance == 0)
            {
                throw new NotImplementedException("Don't go to Mars just yet");
            }
            var hoursToTargetLocation = TimeSpan.FromHours(distance/Speed);
            ArrivalTime = TimeDilation.CalculateTime(Captain.GameLastSeen, Captain.RealLastSeen, DateTime.Now).Add(hoursToTargetLocation);
            return $"{Name} will arrive in orbit above {TargetLocation.Name} on {ArrivalTime}";
        }
    }
}