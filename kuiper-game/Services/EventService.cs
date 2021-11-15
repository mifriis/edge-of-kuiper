using System;
using System.Collections.Generic;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public class EventService : IEventService
    {
        private List<IEvent> _events = new List<IEvent>();
        public void AddEvent(IEvent gameEvent)
        {
            _events.Add(gameEvent);
        }

        public void ExecuteEvents(DateTime eventsBefore)
        {

            foreach(var gameEvent in _events)
            {
                if(gameEvent.EventTime <= eventsBefore)
                {
                    gameEvent.Execute(Array.Empty<string>());
                }
            }
        }

        public void RemoveEvent(IEvent gameEvent)
        {
            _events.Remove(gameEvent);
        }
    }
}