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

        public void Enqueue(IShipEvent @event)
        {
            EventQueue.Add(@event);
            Console.WriteLine(@event.StartEvent());
        }

        public void StatusReport() 
        {
            foreach (var evt in EventQueue.Where(x => x.StartTime.Add(x.TaskDuration) < GameTime.Now()))
            {
                ConsoleWriter.Write(HandleEvent((dynamic)evt));
            }
        }

        private string HandleEvent(MiningScan evt)
        {
            return evt.EndEvent();
        }
    }
}