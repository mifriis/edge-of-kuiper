using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Domain.Ship;
using Kuiper.Systems.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using LamarCodeGeneration.Util;

namespace Kuiper.Services
{
    public class ShipService : IShipService
    {
        public Ship Ship { get; set; }
        private readonly ISolarSystemService _solarSystemService;
        private readonly IEventService _eventService;
        private readonly IGameTimeService _gameTimeService;

        public ShipService(ISolarSystemService solarSystemService, IEventService eventService, IGameTimeService gameTimeService)
        {
            _solarSystemService = solarSystemService;
            _eventService = eventService;
            _gameTimeService = gameTimeService;
        }
        
        public IEnumerable<CelestialBody> GetPossibleDestinations()
        {
            var destinations = new List<CelestialBody>();
            var planets = _solarSystemService.GetBodies().ToList();
            var moons = _solarSystemService.GetSatellites(Ship.CurrentLocation).ToList();
            var asteroids = _solarSystemService.Asteroids;
            destinations.AddRange(planets);
            destinations.AddRange(moons);
            destinations.AddRange(asteroids);
            destinations.Remove(Ship.CurrentLocation);
            return destinations;
        }

        public SetCourseEvent SetCourse(string destination)
        {
            var celestialBody = _solarSystemService.GetBody(destination);
            var asteroid = _solarSystemService.GetAsteroid(destination);
            celestialBody = celestialBody ?? asteroid;
            if(celestialBody == null)
            {
                throw new ArgumentException("Destination not found");
            }

            var possibleDestinations = GetPossibleDestinations().ToList();
            if(!possibleDestinations.Contains(celestialBody))
            {
                throw new ArgumentException("Chosen destination is not possible from this location");
            }
            
            Ship.TargetLocation = celestialBody;
            Ship.Status = ShipStatus.Enroute;
            var travelTime = CalculateTravelTime(celestialBody);
            var deltaV = CalculateDeltaVForJourney(celestialBody);
            var arrivalTime = _gameTimeService.Now().Add(travelTime);
            var gameEvent = new SetCourseEvent() { EventTime = arrivalTime, EventName = "Travel Event", DeltaVSpent = deltaV};
            _eventService.AddEvent(gameEvent);
            return gameEvent;
        }

        public void FinalizeJourney(double deltaVSpent)
        {
            Ship.CurrentLocation = Ship.TargetLocation;
            Ship.TargetLocation = null;
            Ship.Status = Domain.ShipStatus.InOrbit;
            Ship.SpendFuel(deltaVSpent);
        }

        public TimeSpan CalculateTravelTime(CelestialBody destination)
        {
            var distanceKm = _solarSystemService.GetDistanceInKm(Ship.CurrentLocation, destination);
            var secondsToDestination = 2*Math.Sqrt((distanceKm * 1000) / Ship.Acceleration); //Direct Trajectory using Brachistochrone math.
            return TimeSpan.FromSeconds(secondsToDestination);
        }

        public double CalculateDeltaVForJourney(CelestialBody destination)
        {
            var distanceKm = _solarSystemService.GetDistanceInKm(Ship.CurrentLocation, destination);
            var deltaV = 2*Math.Sqrt((distanceKm * 1000) * Ship.Acceleration); //Direct Trajectory using Brachistochrone math. 
            return deltaV;
        }

        public String LookupSolarSystem(DateTime time)
        {
            var minX = -6;
            var maxX = 6;
            var minY = -6;
            var maxY = 6;
            var solarSystemString = "";
            
            var bodies = _solarSystemService.SolarSystem
                .Where(x => x.CelestialBodyType != CelestialBodyType.Moon)
                .Where(x => x.CelestialBodyType != CelestialBodyType.Asteroid);
            var elapsedTime = _gameTimeService.GameStartDate - time;
            var sortedBodies = bodies.OrderBy(p => p.GetPosition(elapsedTime).X);

            for (int t = maxY; t >= minY; t--)
            {
                var totalIndentsOnLine = 0;
                solarSystemString += Math.Abs(t) + ")";
                var firstOnLine = true;
                foreach (var body in sortedBodies)
                {
                    var pos = body.GetPosition(elapsedTime);
                    if (Math.Round(new decimal(pos.Y),0) == t 
                        && Math.Round(new decimal(pos.X),0) <= maxX
                        && Math.Round(new decimal(pos.X),0) >= minX)
                    {
                        var indents = (int)Math.Abs(minX - pos.X);
                        
                        if (!firstOnLine)
                        {
                            var sameLineIdents = indents-totalIndentsOnLine;
                            solarSystemString += (indentBuilder(sameLineIdents) + body.Name.Substring(0, 2));
                            totalIndentsOnLine += sameLineIdents;
                        }
                        
                        if (firstOnLine)
                        {
                            firstOnLine = false;
                            solarSystemString += (indentBuilder(indents) + body.Name.Substring(0, 2));
                            totalIndentsOnLine = indents;
                        }
                    }
                }
                solarSystemString += System.Environment.NewLine;
            }
            
            return solarSystemString;
        }
        
        private string indentBuilder(int numberOfIndents)
        {
            var indents = "";
            for (int i = 1; i <= numberOfIndents; i++)
            {
                indents += "    ";
            }

            return indents;
        }
    }
}