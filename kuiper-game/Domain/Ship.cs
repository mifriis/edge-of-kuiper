using System;

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
        public Location Location { get; set; }

        public string Description { 
            get {
                return $"{Environment.NewLine}{Name}, a {ShipClass} in orbit above {Location.Name}";
            }
        }

        public string ShipStatsDescription { 
            get {
                return $"{Environment.NewLine}Name: {Name}{Environment.NewLine}Class: {ShipClass}{Environment.NewLine}Speed: {Speed} km/h";
            }
        }

        public string LocationDescription { 
            get {
                return $"{Environment.NewLine}{Name} is in orbit above {Location.Name}";
            }
        }
    }
}