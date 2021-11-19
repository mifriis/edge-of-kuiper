using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;
using Kuiper.Domain;
using Kuiper.Systems.Events;
using Lamar;

namespace Kuiper.Tests.Unit.Services
{
    public class ShipShould
    {
        [Fact]
        public void CalculateAccelerationCorrectly()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 100};

            //Act
            var acceleration = ship.Acceleration;

            //Assert
            Assert.Equal(2.857,acceleration,3);
        }

        [Fact]
        public void CalculateAccelerationGsCorrectly()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 100};

            //Act
            var acceleration = ship.AccelerationGs;

            //Assert
            Assert.Equal(0.29,acceleration,2);
        }

        [Fact]
        public void CalculateDeltaVelocityCorrectly()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 100};

            //Act
            var dV = ship.deltaV;

            //Assert
            Assert.Equal(3629632,dV,0);
        }

        [Fact]
        public void DeductFuelBasedOnSpentDVCorrectly()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 100};
            var deltaV = ship.deltaV;

            //Act
            var fuel = ship.SpendFuel(deltaV / 2);

            //Assert
            Assert.Equal(50, fuel);
            Assert.Equal(50, ship.FuelMass);
            Assert.Equal(1966760, ship.deltaV, 0); //Ship has become lighter by expending fuel, capable of slightly more 
        }

        [Fact]
        public void ReturnSpentFuel()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 100};
            var deltaV = ship.deltaV * 0.25;

            //Act
            var fuel = ship.SpendFuel(deltaV);

            //Assert
            Assert.Equal(25, fuel);
            Assert.Equal(75, ship.FuelMass);
        }

        [Fact]
        public void EmptyTheTankOnFullDeltaVSpent()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 100};
            var deltaV = ship.deltaV;

            //Act
            var fuel = ship.SpendFuel(deltaV);

            //Assert
            Assert.Equal(100, fuel);
            Assert.Equal(0, ship.FuelMass);
        }

        [Fact]
        public void NotPossibleToForceSpendingMoreFuelThanAvailiable()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 100};
            var deltaV = ship.deltaV + 1;

            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => ship.SpendFuel(deltaV));
            Assert.Equal(100, ship.FuelMass);
        }

        [Fact]
        public void RefuelWithoutOverFilling()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 50};
            var deltaV = ship.deltaV;

            //Act
            var fuel = ship.Refuel(100);

            //Assert
            Assert.Equal(50, fuel);
            Assert.Equal(100, ship.FuelMass);
        }

        [Fact]
        public void RefuelWithoutFillingTheTank()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("LongLars", engine, 250) { FuelMass = 50};
            var deltaV = ship.deltaV;

            //Act
            var fuel = ship.Refuel(20);

            //Assert
            Assert.Equal(20, fuel);
            Assert.Equal(70, ship.FuelMass);
        }
    }
}