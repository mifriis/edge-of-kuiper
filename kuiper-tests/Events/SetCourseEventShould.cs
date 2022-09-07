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
using Kuiper.Domain.Ship;

namespace Kuiper.Tests.Unit.Services
{
    public class SetCourseEventShould
    {
        [Fact]
        public void ExecuteTheRightServices()
        {
            //Arrange
            var now = DateTime.Now;
            var container = new Mock<IContainer>();
            var shipService = new Mock<IShipService>();
            var ship = new Ship("Johnny5", new ShipEngine(1,1,1,1),1) { CurrentLocation = new CelestialBody() { Name = "Earth"}};
            container.Setup(u => u.GetInstance<IShipService>()).Returns(shipService.Object);
            shipService.SetupGet(u => u.Ship).Returns(ship);
            var deltaVSpent = 100000;
            var gameEvent = new SetCourseEvent() { DeltaVSpent = deltaVSpent };
            
            //Act
            gameEvent.Execute(container.Object);

            //Assert
            shipService.Verify(x => x.FinalizeJourney(deltaVSpent), Times.Exactly(1));
        }
    }
}