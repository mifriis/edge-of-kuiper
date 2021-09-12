using System;
using System.Collections.Generic;
using Kuiper.Services;
namespace Kuiper.Domain
{
    public class MiningScan : IShipEvent
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
            if(DiceFactory.Roller().D100(chance,0))
            {
                var distance = DiceFactory.Random().Next(50000, 1500000);
                var asteroid = new Location("Scanned Asteroid",distance,new List<Location>(), SatteliteType.Asteroid);
                CaptainLocator.Captain.Ship.CurrentLocation.Sattelites.Add(asteroid);
                Locations.Destinations.Add(asteroid);
                return $"Asteroid scanning complete. Found a candidate {distance}km from {CaptainLocator.Captain.Ship.CurrentLocation}. Set a course to begin mining the asteroid.";
            }
            return $"Asteroid scanning complete, but found no asteroids with a yield within the scanners parameters.";

        }
    }
}