using System;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Domain
{
    public class Ship
    {
        private const double EARTH_GRAVITY = 9.80665;
        public Ship(string name, string shipClass, ShipEngine engine, double dryMass)
        {
            Name = name;
            ShipClass = shipClass;
            Engine = engine;
            DryMass = dryMass;
        }

        public string Name { get; }
        public ShipEngine Engine { get; }
        public string ShipClass { get; }
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
        public double DryMass { get; }

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