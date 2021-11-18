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
            Console.Clear(); //There might be remnants in the console from other tests that will impact when many tests are run
            var destinations = new List<CelestialBody>() { new CelestialBody() { Name = "Mars" }, new CelestialBody() { Name = "Sovereign" } };
            var eventService = new Mock<IEventService>();
            var shipService = new Mock<IShipService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var command = new SetCourseCommand(shipService.Object, eventService.Object, gameTimeService.Object);
            var ship = new Ship("LongLars","JazzBaron",2) { TargetLocation = destinations[0], Status = ShipStatus.Enroute};
            shipService.Setup(x => x.GetPossibleDestinations()).Returns(destinations);
            shipService.SetupGet(x => x.Ship).Returns(ship);
            gameTimeService.Setup(u => u.Now()).Returns(DateTime.Now);
            
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader("1");
            Console.SetIn(input);

            //Act
            command.Execute(Array.Empty<string>());
            
            //Assert
            eventService.Verify(x => x.AddEvent(It.IsAny<IEvent>()), Times.Exactly(1));
            shipService.Verify(x => x.SetCourse(It.IsAny<string>()), Times.Exactly(1));
            
        }

        [Fact]
        public void RemoveEventFromEventList()
        {
            //Arrange
            var gameEvent = new SetCourseEvent() { EventName = "Headed for Soverign", EventTime=DateTime.MinValue };
            var gameEvents = new List<IEvent>() { gameEvent };
            var serviceLocator = new Mock<IContainer>();
            var eventService = new EventService(serviceLocator.Object);
            eventService.GameEvents = gameEvents;
            //Act
            eventService.RemoveEvent(gameEvent);

            //Assert
            Assert.Equal(eventService.GameEvents.Count(), 0);
            Assert.False(eventService.GameEvents.Contains(gameEvent));
        }

        [Fact]
        public void ExecuteAllEventsThatHappenedBeforeNow()
        {
            //Arrange
            var gameEventEarlier = new Mock<IEvent>();
            gameEventEarlier.SetupGet(x => x.EventName).Returns("Headed for Sovereign");
            gameEventEarlier.SetupGet(x => x.EventTime).Returns(DateTime.MinValue);
            var gameEventLater = new Mock<IEvent>();
            gameEventLater.SetupGet(x => x.EventName).Returns("Crashlanded on Sovereign");
            gameEventLater.SetupGet(x => x.EventTime).Returns(DateTime.MaxValue);
            var gameEvents = new List<IEvent>() { gameEventEarlier.Object, gameEventLater.Object };
            var serviceLocator = new Mock<IContainer>();
            var eventService = new EventService(serviceLocator.Object);
            eventService.GameEvents = gameEvents;
            //Act
            eventService.ExecuteEvents(DateTime.Now);

            //Assert
            Assert.Equal(eventService.GameEvents.Count(), 1);
            Assert.False(eventService.GameEvents.Contains(gameEventEarlier.Object));
            Assert.True(eventService.GameEvents.Contains(gameEventLater.Object));
            gameEventEarlier.Verify(x => x.Execute(serviceLocator.Object), Times.Exactly(1));
            gameEventLater.Verify(x => x.Execute(serviceLocator.Object), Times.Exactly(0));
        }
    }
}