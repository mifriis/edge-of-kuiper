using System;

public interface IShipEvent
{
    string Name {get;}
    GameTime StartTime {get;}
    TimeSpan TaskDuration {get;}

    string GetPrompt();
}