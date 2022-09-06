using Xunit;
using Kuiper.Domain.CelestialBodies;
using System;
using System.Numerics;
using Kuiper.Domain.Mining;

namespace Kuiper.Tests.Unit.Domain
{
    public class AsteroidShould
    {
        [Fact]
        public void Create() 
        {
            // Arrange
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star);
        
            //Act
            var asteroid = new Asteroid(AsteroidType.M, AsteroidSize.Tiny, 2,2,2,2,star);

            // Assert
            Assert.Equal(asteroid.OrbitRadius, 2);
            Assert.Equal(asteroid.Parent, star);
            Assert.NotNull(asteroid.Satellites);
        }

        
    }
}