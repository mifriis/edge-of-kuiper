using System;

public interface IShipEvent
{
    string Name {get;}
    DateTime StartTime {get;}
    TimeSpan TaskDuration {get;}
    string StartEvent();
    string EndEvent();
}