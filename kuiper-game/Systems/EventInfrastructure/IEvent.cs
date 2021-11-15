using System;

namespace Kuiper.Systems
{   
    public interface IEvent
    {
        DateTime EventTime { get; set; }
        string EventName { get; set; }

        void Execute(string[] args);

    }
}