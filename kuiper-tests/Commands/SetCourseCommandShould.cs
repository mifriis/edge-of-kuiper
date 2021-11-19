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
using Kuiper.Systems;
using System.IO;

namespace Kuiper.Tests.Unit.Services
{
    public class SetCourseCommandShould
    {
        [Fact]
        public void SetCourseAndAddEventOnExecute()
        {
            //Arrange
            var now = DateTime.Now;
            Console.Clear(); //There might be remnants in the console from other tests that will impact when many tests are run
            var destinations = new List<CelestialBody>() { new CelestialBody() { Name = "Mars" }, new CelestialBody() { Name = "Sovereign" } };
            var eventService = new Mock<IEventService>();
            var shipService = new Mock<IShipService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var travelTime = TimeSpan.FromDays(255);
            var command = new SetCourseCommand(shipService.Object, eventService.Object, gameTimeService.Object);
            var ship = new Ship("LongLars","JazzBaron",new ShipEngine(10000,3,1000000,1100000), 250) { TargetLocation = destinations[0], Status = ShipStatus.Enroute, FuelMass = 100};
            shipService.Setup(x => x.GetPossibleDestinations()).Returns(destinations);
            shipService.SetupGet(x => x.Ship).Returns(ship);
            shipService.Setup(u => u.CalculateTravelTime(destinations[0])).Returns(travelTime);
            gameTimeService.Setup(u => u.Now()).Returns(now);
            
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader("1");
            Console.SetIn(input);

            //Act
            command.Execute(Array.Empty<string>());
            
            //Assert
            eventService.Verify(x => x.AddEvent(It.Is((SetCourseEvent e) => e.EventTime == now + travelTime)), Times.Exactly(1));
            shipService.Verify(x => x.SetCourse(It.IsAny<string>()), Times.Exactly(1));        
        }
    }
}