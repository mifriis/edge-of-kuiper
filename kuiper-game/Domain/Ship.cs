using System;
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
        public GameTime ArrivalTime { get; set;}

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
    }
}