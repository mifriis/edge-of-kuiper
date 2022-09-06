using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;
using Kuiper.Domain;
using Kuiper.Domain.Mining;
using Kuiper.Systems.Events;
using Lamar;
using Kuiper.Domain.Ship;

namespace Kuiper.Tests.Unit.Services
{
    public class ScanForAsteroidsEventShould
    {
        [Fact]
        public void ExecuteTheRightServices()
        {
            //Arrange
            var now = DateTime.Now;
            var container = new Mock<IContainer>();
            var miningService = new Mock<IMiningService>();
            var solarSystemService = new Mock<ISolarSystemService>();
            var star = CelestialBody.Create("Sun", 0, null, CelestialBodyType.Star);
            
            container.Setup(u => u.GetInstance<IMiningService>()).Returns(miningService.Object);
            container.Setup(u => u.GetInstance<ISolarSystemService>()).Returns(solarSystemService.Object);
            
            solarSystemService.Setup(x => x.GetStar()).Returns(star);
            
            var gameEvent = new ScanForAsteroidsEvent() { EventTime = DateTime.Now.AddDays(1), EventName = "ScanForAsteroid"};
            
            //Act
            gameEvent.Execute(container.Object);

            //Assert
            solarSystemService.Verify(x => x.GetStar(), Times.Exactly(1));
            solarSystemService.Verify(x => x.AddAsteroid(It.IsAny<Asteroid>()), Times.Exactly(1));
            solarSystemService.Verify(x => x.AddCelestialBody(It.IsAny<Asteroid>()), Times.Exactly(0));
            solarSystemService.Verify(x => x.AddCelestialBody(It.IsAny<CelestialBody>()), Times.Exactly(0));
        }
    }
}