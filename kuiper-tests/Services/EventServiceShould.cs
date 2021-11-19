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
    public class EventServiceShould
    {
        [Fact]
        public void AddEventToEventList()
        {
            //Arrange
            var gameEvent = new SetCourseEvent() { EventName = "Headed for Soverign", EventTime=DateTime.MinValue };
            var serviceLocator = new Mock<IContainer>();
            var eventService = new EventService(serviceLocator.Object);
            
            //Act
            eventService.AddEvent(gameEvent);

            //Assert
            Assert.Equal(eventService.GameEvents.Count(), 1);
            Assert.Equal(eventService.GameEvents.FirstOrDefault(), gameEvent);
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