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
            var ship = new Ship("LongLars","Boofy", engine, 250) { FuelMass = 100};

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
            var ship = new Ship("LongLars","Boofy", engine, 250) { FuelMass = 100};

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
            var ship = new Ship("LongLars","Boofy", engine, 250) { FuelMass = 100};

            //Act
            var dV = ship.deltaV;

            //Assert
            Assert.Equal(3629632,dV,0);
        }
    }
}