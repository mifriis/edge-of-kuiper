using Xunit;
using Kuiper.Systems;
using kuiper.Domain.CelestialBodies;
using System;
using System.Drawing;

namespace Kuiper.Tests.Unit.Domain
{
    public class CelestialBodyShould
    {
        [Fact]
        public void CreateAStar()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);

            //Act

            //Assert
            Assert.IsType<Star>(star);
        }

        [Fact]
        public void CreateAPlanet()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            var planet = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet);

            //Act

            //Assert
            Assert.IsType<Planet>(planet);
        }

        [Fact]
        public void CreateAMoon()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            var planet = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet);
            var moon = CelestialBody.Create("Luna", 0.00257356604f, 1.022f, 125, planet, CelestialBodyType.Moon);

            //Act

            //Assert
            Assert.IsType<Moon>(moon);
        }

        [Fact]
        public void CreateAGasGiant()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            var gasGiant = CelestialBody.Create("Saturn", 9.582f, 9.7f, 345, star, CelestialBodyType.GasGiant);
            //Act

            //Assert
            Assert.IsType<GasGiant>(gasGiant);
        }

        [Fact]
        public void CreateADwarfPlanet()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            var dwarfPlanet = CelestialBody.Create("Pluto", 39.48f, 4.74f, 260, star, CelestialBodyType.DwarfPlanet);
            //Act

            //Assert
            Assert.IsType<DwarfPlanet>(dwarfPlanet);
        }

        [Fact]
        public void SetCorrectName()
        {
            //Arrange
            var name = "TestName";
            var radius = 12f;
            var velocity = 34f;
            var degrees = 90f;

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet);

            //Assert
            Assert.Equal(name, dwarfPlanet.Name);
        }

        [Fact]
        public void SetCorrectOrbitRadius()
        {
            //Arrange
            var name = "TestName";
            var radius = 12f;
            var velocity = 34f;
            var degrees = 90f;

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet);

            //Assert
            Assert.Equal(radius, dwarfPlanet.OrbitRadius);
        }

        [Fact]
        public void SetCorrectOrbitVelocity()
        {
            //Arrange
            var name = "TestName";
            var radius = 12f;
            var velocity = 34f;
            var degrees = 90f;

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet);

            //Assert
            Assert.Equal(velocity, dwarfPlanet.Velocity);
        }

        [Fact]
        public void SetCorrectOriginDegrees()
        {
            //Arrange
            var name = "TestName";
            var radius = 12f;
            var velocity = 34f;
            var degrees = 90f;

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet);

            //Assert
            Assert.Equal(degrees, dwarfPlanet.OriginDegrees);
        }

        [Fact]
        public void AddBodyAsSatelliteIfItHasAParent()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            var planet = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet);

            //Act

            //Assert
            Assert.Single(star.Satellites);
            Assert.Contains(planet, star.Satellites);
        }

        [Fact]
        public void ReturnItsInitialPositionInSpace() 
        {
            // Arrange
            var startingPoint = new PointF(1f, 1f);
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
            var earth = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet);

            //Act
            var results = earth.Position;

            Assert.Equal(startingPoint, results);

        }
    }
}