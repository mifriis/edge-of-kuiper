using System;
using System.Linq;
using Kuiper.Systems;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        
        [JsonIgnore]
        private Captain Captain {get;}
        public string Name { get; }

        public string ShipClass { get; }
        public int Speed { get; }
        public Location CurrentLocation { get; set; }
        public Location TargetLocation { get; set; }
        public List<IShipEvent> EventQueue { get; set;} = new List<IShipEvent>();

        public ShipStatus Status { get; set; }

        public string Description 
        { 
            get {
                if(Status == ShipStatus.Enroute)
                {
                    return $"{Name}, a {ShipClass} {ShipStatusExtensions.ToFriendlyString(Status)} {TargetLocation.Name}";
                }
                return $"{Name}, a {ShipClass} {ShipStatusExtensions.ToFriendlyString(Status)} {CurrentLocation.Name}";
            }
        }

        public string ShipStatsDescription 
        { 
            get {
                return $"Name: {Name}{Environment.NewLine}Class: {ShipClass}{Environment.NewLine}Speed: {Speed} km/h";
            }
        }

        public string LocationDescription 
        { 

            get {
                if(Status == ShipStatus.Enroute)
                {
                    return $"{Name} {ShipStatusExtensions.ToFriendlyString(Status)} {TargetLocation.Name}";
                }
                return $"{Name} {ShipStatusExtensions.ToFriendlyString(Status)} {CurrentLocation.Name}";
            }
        }

        public void Enqueue(IShipEvent @event)
        {
            EventQueue.Add(@event);
            Console.WriteLine(@event.GetPrompt());
        }

        public void StatusReport() 
        {
            foreach (var evt in EventQueue.Where(x => x.StartTime.GetCurrentInGameTime().Add(x.TaskDuration) < Captain.StartTime.GetCurrentInGameTime()))
            {
                HandleEvent((dynamic)evt);
            }
        }

        private void HandleEvent(CourseSet courseSet)
        {
            CurrentLocation = TargetLocation;
            TargetLocation = null;
            Status = ShipStatus.InOrbit;
            //ConsoleWriteLine We have arrived at {CurrentLocation}
            //Press 'Enter' to continue
        }
    }
}