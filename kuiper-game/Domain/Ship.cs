using System;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Domain
{
    public class Ship
    {
        private const double EARTH_GRAVITY = 9.80665;
        public Ship(string name, string shipClass, int speed)
        {
            Name = name;
            ShipClass = shipClass;
            Speed = speed;
        }

        public string Name { get; }

        public ShipEngine Engine { get; set; }

        public string ShipClass { get; }
        public int Speed { get; }
        public CelestialBody CurrentLocation { get; set; }
        public CelestialBody TargetLocation { get; set; }
        public ShipStatus Status { get; set; }
        public double WetMass 
        { 
            get 
            {
                return FuelMass + DryMass;
            }
        }
        public double FuelMass { get; set;}
        public double DryMass { get; set;}

        public double deltaV 
        {
            get
            {
                return Engine.ExhaustVelocity * Math.Log(WetMass / DryMass);
            }
        }

        public double Acceleration
        {
            get
            {
                return Engine.Thrust / (WetMass * 1000);
            }
        }

        public double AccelerationGs
        {
            get
            {
                return Acceleration / EARTH_GRAVITY;
            }
        }
    }
}