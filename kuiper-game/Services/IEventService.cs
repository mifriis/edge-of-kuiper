using System;
using System.Collections.Generic;
using Kuiper.Systems;
using Kuiper.Systems.Events;

namespace Kuiper.Services
{
    public interface IEventService
    {
        void AddEvent(IEvent gameEvent);
        void RemoveEvent(IEvent gameEvent);
        void ExecuteEvents(DateTime eventsBefore);
        List<IEvent> GameEvents { get; set;}
        

        
    }
}