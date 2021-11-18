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
    public class ShipEngineShould
    {
        [Fact]
        public void CalculateThrustToWeightRationCorrectly()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,60,800); 

            //Act
            var twr = engine.ThrustToWeightRatio;

            //Assert
            Assert.InRange(twr,2.03,2.04);
        }

        [Fact]
        public void CalculateExhaustVelocityCorretly()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,60,800); 

            //Act
            var vex = engine.ExhaustVelocity;

            //Assert
            Assert.Equal(7845.32, vex);
        }

        [Fact]
        public void CalculateDeltaVForStarship()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,60,800); 
            var fuelWeight = 50;
            var dryMass = 100;
            double fullMass = fuelWeight + dryMass;
            double ratio = fullMass / dryMass;
            //Act
            var dV = engine.ExhaustVelocity * Math.Log(ratio);

            //Assert
            Assert.Equal(3181,(int)dV);
        }

        [Fact]
        public void MatchRoughlyTorchship()
        {
            //Arrange
            var engine = new ShipEngine(10000,3,1000000,1100000); 
            var fuelWeight = 100;
            var dryMass = 250;
            double wetMass = fuelWeight + dryMass;
            double ratio = wetMass / dryMass;
            //Act
            var dV = engine.ExhaustVelocity * Math.Log(ratio); //meters pr second in changed velocity
            var acceleration = engine.Thrust / (wetMass * 1000); //meters pr second^2
            var distanceMeters = (Int64)245749658 * 1000;
            var dVForMarsJourney = 2*Math.Sqrt(distanceMeters * acceleration);
            var timeToMars = 2*Math.Sqrt(distanceMeters / acceleration);
            var hoursToMars = timeToMars / 60 / 60;
            var remainingdeltaV = dV - dVForMarsJourney;
            var gs = acceleration / 9.80;

            //Assert
            Assert.Equal(3629632,dV, 0);
            Assert.Equal(2.857,acceleration,3);
            Assert.Equal(1675878,dVForMarsJourney,0);
            Assert.Equal(163, hoursToMars,0);
        }
    }
}