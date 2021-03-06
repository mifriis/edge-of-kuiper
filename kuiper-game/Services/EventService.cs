using System;
using System.Collections.Generic;
using Kuiper.Systems;
using Kuiper.Systems.Events;
using Lamar;

namespace Kuiper.Services
{
    public class EventService : IEventService
    {
        private readonly IContainer _serviceLocator;

        public List<IEvent> GameEvents { get; set; }

        public EventService(IContainer serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public void AddEvent(IEvent gameEvent)
        {
            if(GameEvents == null)
            { 
                GameEvents = new List<IEvent>();
            }
            GameEvents.Add(gameEvent);
        }

        public void ExecuteEvents(DateTime eventsBefore)
        {
            var executedEvents = new List<IEvent>();
            if(GameEvents != null)
            {
                foreach(var gameEvent in GameEvents)
                {
                    if(gameEvent.EventTime <= eventsBefore)
                    {
                        executedEvents.Add(gameEvent);
                        gameEvent.Execute(_serviceLocator);

                    }
                }
                foreach (var gameEvent in executedEvents)
                {
                    RemoveEvent(gameEvent);
                }
            }
        }

        public void RemoveEvent(IEvent gameEvent)
        {
            if(GameEvents != null)
            { 
                GameEvents.Remove(gameEvent);    
            }
        }
    }
}