using System;
using System.Collections.Generic;
using System.Linq;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Domain.Ship
{
    public class Ship
    {
        public Ship(string name, IShipEngine engine, double shipMass)
        {
            Name = name;
            Engine = engine;
            ShipMass = shipMass;
        }

        public string Name { get; set; }
        public IShipEngine Engine { get; set; }
        public CelestialBody CurrentLocation { get; set; }
        public CelestialBody TargetLocation { get; set; }
        public ShipStatus Status { get; set; }
        public double WetMass
        {
            get
            {
                return FuelMass +
                       DryMass;
            }
        }
        public double FuelMass { get; set; }

        public double DryMass
        {
            get
            {
                return ShipMass +
                       Engine.Mass +
                       Modules.Sum(x => x.Mass);
            }
        }

        public double ShipMass { get; set; }
        public IEnumerable<IShipModule> Modules { get; set; }

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
                return Acceleration / Physics.STANDARD_GRAVITY;
            }
        }

        public double SpendFuel(double deltaVToSpend)
        {
            if (deltaVToSpend > deltaV)
            {
                throw new ArgumentOutOfRangeException("Not possible to spend more fuel than is availiable");
            }
            if (deltaVToSpend == deltaV) //Avoiding divideByZero
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
            var maxFuel = Modules.Where(x => x.Type == ModuleType.FuelTank)
                                        .OfType<FuelTank>()
                                        .Sum(x => x.FuelMass);
            var fuel = FuelMass + tonsRefueled;
            if (fuel > maxFuel)
            {
                FuelMass = maxFuel;
                return maxFuel;
            }
            FuelMass = fuel;
            return tonsRefueled;
        }
    }
}