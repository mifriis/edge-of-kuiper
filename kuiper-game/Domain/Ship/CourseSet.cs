using System;
using System.Linq;
using Kuiper.Domain;
using Newtonsoft.Json;

class CourseSet : IShipEvent
{
    public CourseSet(Ship ship, Location source, Location destination, GameTime startTime)
    {
        Source = source;
        Destination = destination;
        StartTime = startTime;
        Ship = ship;

        if(destination == source)
        {
            return; // $"{ship.Name} is already in orbit above {ship.CurrentLocation.Name}";
        }
        if(destination == source)
        {
            return; // $"{ship.Name} is already enroute to {ship.TargetLocation.Name}";
        }
        
        long distance = 0;
        if(destination.Sattelites.Contains(source))
        {
            //Travel from a moon to parent
            distance = source.OrbitalRadius;
            
        }
        if(source.Sattelites.Contains(destination))
        {
            //Travel from a parent to one of it's moons
            distance = destination.OrbitalRadius;
        }
        if(distance == 0)
        {
            throw new NotImplementedException("Don't go to Mars just yet");
        }
        TaskDuration = TimeSpan.FromHours(distance/ship.Speed);
        

    }

    public string Name => "Course set";

    public string Description => $"Ship has started journey towards {Destination.Name}";

[JsonIgnore]
public Ship Ship {get;}
    public Location Source { get; }
    public Location Destination { get; }

    public GameTime StartTime {get;}

    public TimeSpan TaskDuration {get;}

    public string GetPrompt()
    {
        return $"{Ship.Name} will arrive in orbit above {Destination.Name} on {StartTime.GetCurrentInGameTime().Add(TaskDuration)}";

    }
}