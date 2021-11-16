using System;

namespace Kuiper.Systems.Events
{   
    public interface IEvent
    {
        DateTime EventTime { get; }
        string EventName { get; }

        void Execute(string[] args);

    }
}