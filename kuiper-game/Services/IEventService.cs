using System;
using System.Collections.Generic;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public interface IEventService
    {
        void AddEvent(IEvent gameEvent);
        void RemoveEvent(IEvent gameEvent);
        void ExecuteEvents(DateTime eventsBefore);

        
    }
}