using System;
using System.Linq;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Domain.Mining;
using Kuiper.Services;
using Lamar;

namespace Kuiper.Systems.Events
{
    public class ScanForAsteroidsEvent : IEvent
    {
        public DateTime EventTime { get; set; }
        public string EventName { get; set; }
        
        public void Execute(IContainer serviceLocator)
        {
            var miningService = serviceLocator.GetInstance<IMiningService>();
            var solarSystemService = serviceLocator.GetInstance<ISolarSystemService>();
            var parent = solarSystemService.GetStar();

            var dummyCelestialBody =
                CelestialBody.Create("", 2.6, parent, CelestialBodyType.Asteroid);
            var asteroid = new Asteroid(AsteroidType.S, AsteroidSize.Tiny, 10, dummyCelestialBody.OrbitRadius,dummyCelestialBody.OriginDegrees,dummyCelestialBody.Velocity, dummyCelestialBody.Parent);
            solarSystemService.AddAsteroid(asteroid);

            ConsoleWriter.Write(EventTime.ToUniversalTime() +  " A " + asteroid.AsteroidSize + " " + asteroid.AsteroidType + " class asteroid found with an estimated yield of " + asteroid.Yield);
            ConsoleWriter.Write(EventTime.ToUniversalTime() +  " It has been added to the list of prospected asteroids.");
        }
    }
} 