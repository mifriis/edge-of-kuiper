using System;
using System.Collections.Generic;
using Kuiper.Systems;
using Kuiper.Systems.Events;

namespace Kuiper.Services
{
    public class EventService : IEventService
    {
        public List<IEvent> GameEvents { get; set; }

        public void AddEvent(IEvent gameEvent)
        {
            GameEvents.Add(gameEvent);
        }

        public void ExecuteEvents(DateTime eventsBefore)
        {

            foreach(var gameEvent in GameEvents)
            {
                if(gameEvent.EventTime <= eventsBefore)
                {
                    gameEvent.Execute(Array.Empty<string>());
                }
            }
        }

        public void RemoveEvent(IEvent gameEvent)
        {
            GameEvents.Remove(gameEvent);
        }
    }
}