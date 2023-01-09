using Xunit;
using Moq;
using Kuiper.Services;
using System.Linq;
using System.Collections.Generic;
using System;
using Kuiper.Domain.Mining;

namespace Kuiper.Tests.Unit.Services
{
    public class MiningServiceShould
    {
        [Fact]
        public void ScannedAsteroidsReturnsAsteroid()
        {
            //Arrange
            var asteroid = new Asteroid(AsteroidType.M, AsteroidSize.Tiny, 2, 2, 2, 2, null);

            var solarSystemService = new Mock<ISolarSystemService>();
            solarSystemService.Setup(x => x.Asteroids).Returns(new List<Asteroid>() { asteroid });

            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7, 0, 0, 0));

            var eventService = new Mock<IEventService>();

            var miningService = new MiningService(solarSystemService.Object, eventService.Object, gameTimeService.Object);

            //Act
            var asteroids = miningService.ScannedAsteroids();

            //Assert
            Assert.NotNull(asteroids);
            Assert.Single(asteroids);
            Assert.Equal(asteroid.AsteroidType, asteroids.First().AsteroidType);
        }

        [Fact]
        public void ScanForAsteroidsReturnsEvent()
        {
            //Arrange
            var solarSystemService = new Mock<ISolarSystemService>();

            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7, 0, 0, 0));

            var eventService = new Mock<IEventService>();

            var miningService = new MiningService(solarSystemService.Object, eventService.Object, gameTimeService.Object);

            //Act
            var scanForAsteroidsEvent = miningService.ScanForAsteroids();

            //Assert
            Assert.NotNull(scanForAsteroidsEvent);
            Assert.Equal("Asteroid Scanning", scanForAsteroidsEvent.EventName);
            //test
        }
    }
}
