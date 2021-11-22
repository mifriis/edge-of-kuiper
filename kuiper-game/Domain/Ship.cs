using System;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Domain
{
    public class Ship
    {
        private const double EARTH_GRAVITY = 9.80665;
        public Ship(string name, IShipEngine engine, double dryMass)
        {
            Name = name;
            Engine = engine;
            DryMass = dryMass;
        }

        public string Name { get; }
        public IShipEngine Engine { get; }
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

        public double SpendFuel(double deltaVToSpend)
        {
            if(deltaVToSpend > deltaV)
            {
                throw new ArgumentOutOfRangeException("Not possible to spend more fuel than is availiable");
            }
            if(deltaVToSpend == deltaV) //Avoiding divideByZero
            {
                FuelMass = 0;
                return 100;
            }
            var fraction = deltaVToSpend / deltaV;
            var fuelSpent = FuelMass * fraction;
            FuelMass = FuelMass - fuelSpent;
            return fuelSpent;        
        }

        public double Refuel(double tonsRefueled)
        {
            var fuel = FuelMass + tonsRefueled;
            if(fuel > 100)
            {
                FuelMass = 100;
                return tonsRefueled - (fuel - 100);
            }
            FuelMass = fuel;
            return tonsRefueled;
            
        }
    }
}