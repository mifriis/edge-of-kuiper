using System;

namespace Kuiper.Systems.Events
{   
    public interface IEvent
    {
        DateTime EventTime { get; set; }
        string EventName { get; set; }

        void Execute(string[] args);

    }
}