using Xunit;
using Kuiper.Domain.CelestialBodies;
using System;
using System.Numerics;

namespace Kuiper.Tests.Unit.Domain
{
    public class CelestialBodyShould
    {
        [Fact]
        public void CreateAStar()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");

            //Act

            //Assert
            Assert.Equal(star.CelestialBodyType, CelestialBodyType.Star);
        }

        [Fact]
        public void CreateAPlanet()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var planet = CelestialBody.Create("Earth", 1.0, 29.8, 170, star, CelestialBodyType.Planet,"White");

            //Act

            //Assert
            Assert.Equal(planet.CelestialBodyType, CelestialBodyType.Planet);
        }

        [Fact]
        public void CreateAMoon()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var planet = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet,"White");
            var moon = CelestialBody.Create("Luna", 0.00257356604f, 1.022f, 125, planet, CelestialBodyType.Moon,"White");

            //Act

            //Assert
            Assert.Equal(moon.CelestialBodyType, CelestialBodyType.Moon);
        }

        [Fact]
        public void CreateAGasGiant()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var gasGiant = CelestialBody.Create("Saturn", 9.582f, 9.7f, 345, star, CelestialBodyType.GasGiant,"White");
            //Act

            //Assert
            Assert.Equal(gasGiant.CelestialBodyType, CelestialBodyType.GasGiant);
        }

        [Fact]
        public void CreateADwarfPlanet()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var dwarfPlanet = CelestialBody.Create("Pluto", 39.48f, 4.74f, 260, star, CelestialBodyType.DwarfPlanet,"White");
            //Act

            //Assert
            Assert.Equal(dwarfPlanet.CelestialBodyType, CelestialBodyType.DwarfPlanet);
        }

        [Fact]
        public void SetCorrectName()
        {
            //Arrange
            var name = "TestName";
            var radius = 12f;
            var velocity = 34f;
            var degrees = 90f;

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet,"White");

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

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet,"White");

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

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet,"White");

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

            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet,"White");

            //Assert
            Assert.Equal(degrees, dwarfPlanet.OriginDegrees);
        }

        [Fact]
        public void AddBodyAsSatelliteIfItHasAParent()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var planet = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet,"White");

            //Act

            //Assert
            Assert.Single(star.Satellites);
            Assert.Contains(planet, star.Satellites);
        }

        [Fact]
        public void ReturnItsInitialPositionInSpace() 
        {
            // Arrange
            var startingPoint = new Vector2(-0.9848077f, 0.17364818f);
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var earth = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet,"White");

            //Act
            var results = earth.GetPosition(new TimeSpan(0));

            // Assert
            Assert.Equal(startingPoint, results);
        }

        [Fact]
        public void ReturnItsPositionInSpaceAtAGivenTime() 
        {
            // Arrange
            var currentPoint = new Vector2(-0.9917729f, 0.14527623f);
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var earth = CelestialBody.Create("Earth", 1.0f, 29.8f, 170, star, CelestialBodyType.Planet,"White");
            var moon = CelestialBody.Create("Luna", 0.00257356604f, 1.022f, 125, earth, CelestialBodyType.Moon,"White");

            //Act
            var results = moon.GetPosition(new TimeSpan(42,0,0));

            // Assert
            Assert.Equal(currentPoint, results);
        }

        [Fact]
        public void CreateBodyWithVelocity() 
        {
            // Arrange
            var currentPoint = new Vector2(-0.9917729f, 0.14527623f);
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
        
            //Act
            var earth = CelestialBody.Create("Earth", 1.0f, 170, star, CelestialBodyType.Planet);

            // Assert
            Assert.NotNull(earth.Velocity);
        }

        [Fact]
        public void CreateBodyWithOriginDegrees() 
        {
            // Arrange
            var currentPoint = new Vector2(-0.9917729f, 0.14527623f);
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star, "White");
        
            //Act
            var earth = CelestialBody.Create("Earth", 1.0f, star, CelestialBodyType.Planet);

            // Assert
            Assert.NotNull(earth.OriginDegrees);
        }
        
        [Fact]
        public void DontAddBodyAsSatelliteIfItsAnAsteroid()
        {
            //Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star,"White");
            var asteroid = CelestialBody.Create("tp01", 1.0f, 29.8f, 170, star, CelestialBodyType.Asteroid,"White");

            //Act

            //Assert
            Assert.Empty(star.Satellites);
            Assert.DoesNotContain(asteroid, star.Satellites);
        }
    }
}