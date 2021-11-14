using Kuiper.Domain.CelestialBodies;

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
        public CelestialBody CurrentLocation { get; set; }
        public CelestialBody TargetLocation { get; set; }
    }
}