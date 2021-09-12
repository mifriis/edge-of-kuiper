using System;
using System.Collections.Generic;
using System.Linq;
using Kuiper.Systems;

namespace Kuiper.Domain
{
    public class Ship
    {
        public Ship(string name, string shipClass, int speed)
        {
            Name = name;
            ShipClass = shipClass;
            Speed = speed;
        }

        public string Name { get; }

        public string ShipClass { get; }
        public int Speed { get; }
        public Location CurrentLocation { get; set; }
        public Location TargetLocation { get; set; }
        public DateTime ArrivalTime { get; set;}
        public List<IShipEvent> EventQueue { get; set;} = new List<IShipEvent>();

        public ShipStatus Status { get; set; }

        public string Description { 
            get {
                if(Status == ShipStatus.Enroute)
                {
                    return $"{Name}, a {ShipClass} {ShipStatusExtensions.ToFriendlyString(Status)} {TargetLocation.Name}";
                }
                return $"{Name}, a {ShipClass} {ShipStatusExtensions.ToFriendlyString(Status)} {CurrentLocation.Name}";
            }
        }

        public string ShipStatsDescription { 
            get {
                return $"Name: {Name}{Environment.NewLine}Class: {ShipClass}{Environment.NewLine}Speed: {Speed} km/h";
            }
        }

        public string LocationDescription { 
            get {
                if(Status == ShipStatus.Enroute)
                {
                    return $"{Name} {ShipStatusExtensions.ToFriendlyString(Status)} {TargetLocation.Name}";
                }
                return $"{Name} {ShipStatusExtensions.ToFriendlyString(Status)} {CurrentLocation.Name}";
            }
        }

        public string SetCourse(Location target)
        {
            if(target == CurrentLocation)
            {
                return $"{Name} is already in orbit above {CurrentLocation.Name}";
            }
            if(target == TargetLocation)
            {
                return $"{Name} is already enroute to {TargetLocation.Name}";
            }
            Status = ShipStatus.Enroute;
            TargetLocation = target;
            long distance = 0;

            if(target.Satellites.Contains(CurrentLocation))
            {
                //Travel from a moon to parent
                distance = CurrentLocation.OrbitalRadius;
                
            }
            if(CurrentLocation.Satellites.Contains(target))
            {
                //Travel from a parent to one of it's moons
                distance = target.OrbitalRadius;
            }
            if(distance == 0)
            {
                throw new NotImplementedException("Don't go to Mars just yet");
            }
            var hoursToTargetLocation = distance/Speed;
            ArrivalTime = GameTime.Now() + TimeSpan.FromHours(hoursToTargetLocation);
            var evt = new CourseSet(GameTime.Now(), TimeSpan.FromHours(hoursToTargetLocation));
            Enqueue(evt);
            return evt.StartEvent();
        }

        public string ScanForAsteroids()
        {
            if(Status == ShipStatus.InOrbit)
            {
                var evt = new MiningScan(GameTime.Now());
                Enqueue(evt);
                return evt.StartEvent();
            }
            return $"Ship must be in orbit around a stabile location to scan for asteroids";
        }

        public string MineAsteroid(int durationHours)
        {
            if(Status == ShipStatus.InOrbit && CurrentLocation.SatelliteType == SatelliteType.Asteroid)
            {
                var evt = new MineAsteroid(GameTime.Now(), TimeSpan.FromHours(durationHours));
                Enqueue(evt);
                return evt.StartEvent();
            }
            return $"Ship must be in orbit around an asteroid to begin mining";
        }

        public void Enqueue(IShipEvent evt)
        {
            EventQueue.Add(evt);
            Console.WriteLine(evt.StartEvent());
        }

        public void StatusReport() 
        {
            foreach (var evt in EventQueue.ToList().Where(x => x.StartTime.Add(x.TaskDuration) < GameTime.Now()))
            {
                ConsoleWriter.Write(HandleEvent((dynamic)evt));
            }
        }

        private string HandleEvent(MiningScan evt)
        {
            EventQueue.Remove(evt);
            return evt.EndEvent();
        }

        private string HandleEvent(CourseSet evt)
        {
            CurrentLocation = TargetLocation;
            TargetLocation = null;
            Status = ShipStatus.InOrbit;
            EventQueue.Remove(evt);
            return evt.EndEvent();
        }

        private string HandleEvent(MineAsteroid evt)
        {
            EventQueue.Remove(evt);
            return evt.EndEvent();
        }
    }
}