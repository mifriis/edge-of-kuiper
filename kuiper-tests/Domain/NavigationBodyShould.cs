using Xunit;
using Kuiper.Domain.CelestialBodies;
using System;
using System.Numerics;
using Kuiper.Domain.Navigation;

namespace Kuiper.Tests.Unit.Domain
{
    public class NavigationBodyShould
    {
        
        [Fact]
        public void CreateNavigationBodyWithTheRightCoordinate() 
        {
            // Arrange
            var name = "TestName";
            var radius = 12f;
            var velocity = 34f;
            var degrees = 90f;
            
            var now = new DateTime(2023, 10, 10);
            var star = CelestialBody.Create("Sun", 0, 0, 0, null, CelestialBodyType.Star, "White");
            var timeSpan = now.AddYears(10) - now;
            //Act
            var dwarfPlanet = CelestialBody.Create(name, radius, velocity, degrees, star, CelestialBodyType.DwarfPlanet,"White");
            var navBody = new NavigationBody(dwarfPlanet, timeSpan, 6, 18);

            // Assert
            Assert.Equal(32, navBody.NormalisedCoordinate.X);
            Assert.Equal(12, navBody.NormalisedCoordinate.Y);
        }
    }
}