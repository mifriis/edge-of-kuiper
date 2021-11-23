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
    }
}