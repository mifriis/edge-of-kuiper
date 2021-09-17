using System;
using System.Linq;
using Kuiper.Domain;
using Newtonsoft.Json;

class MiningScan : IShipEvent
{
    public MiningScan(Ship ship, DateTime startTime)
    { 
        StartTime = startTime;
        Ship = ship;
        TaskDuration = TimeSpan.FromHours(7);
    }

    public string Name => "Course set";

    public string Description => $"Ship has started journey towards {Destination.Name}";

    [JsonIgnore]
    public Ship Ship {get;}
    public Location Source { get; }
    public Location Destination { get; }

    public DateTime StartTime {get;}

    public TimeSpan TaskDuration {get;}

    public string GetPrompt()
    {
        return $"{Ship.Name} will finish scanning at {GameTime.Now().Add(TaskDuration)} ";
    }
}