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
            return $"{SolarSystemLocator.SolarSystem.Captain.Ship.Name} will finish scanning at {GameTime.Now().Add(TimeSpan.FromHours(7))} ";
        }

        public string EndEvent()
        {
            if(DiceFactory.Roller().D100(chance,0))
            {
                var asteroid = CelestialBodies.Asteroid.Create("Scanned Asteroid",3.2, SolarSystemLocator.SolarSystem.SolarSystemService.GetBody("Sun"),CelestialBodies.CelestialBodyType.Asteroid );
                var distance = SolarSystemLocator.SolarSystem.SolarSystemService.GetDistanceInAu(
                    SolarSystemLocator.SolarSystem.Captain.Ship.CurrentLocation,
                    asteroid
                );
                return $"Asteroid scanning complete. Found a candidate {distance}km from {SolarSystemLocator.SolarSystem.Captain.Ship.CurrentLocation}. Set a course to begin mining the asteroid.";
            }
            return $"Asteroid scanning complete, but found no asteroids with a yield within the scanners parameters.";

        }
    }
}