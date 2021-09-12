using System;
using System.Collections.Generic;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Services;
using Newtonsoft.Json;

class MiningScan : IShipEvent
{
    private const int chance = 80;
    public MiningScan(DateTime startTime)
    { 
        StartTime = startTime;
        TaskDuration = TimeSpan.FromHours(7);
    }

    public string Name => "Asteroid Scan";

    public string Description => $"Scan the local planetary area for valuable asteroids";

    public DateTime StartTime {get;}

    public TimeSpan TaskDuration {get;}

    public string StartEvent()
    {
        return $"{CaptainLocator.Captain.Ship.Name} will finish scanning at {GameTime.Now().Add(TaskDuration)} ";
    }

    public string EndEvent()
    {
        var seed = new Random().Next(1,100);
        if(chance > seed)
        {
            var distance = new Random().Next(50000, 1500000);
            var asteroid = new Location("Scanned Asteroid",distance,new List<Location>(), SatteliteType.Asteroid);
            Locations.Earth.Sattelites.Add(asteroid);
            Locations.Destinations.Add(asteroid);
            return $"Asteroid scanning complete. Found a candidate {distance}km from {CaptainLocator.Captain.Ship.CurrentLocation}. Set a course to begin mining the asteroid.";
        }
        return $"Asteroid scanning complete, but found no asteroids with a yield within the scanners parameters.";

    }
}