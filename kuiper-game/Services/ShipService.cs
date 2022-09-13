using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Domain.Ship;
using Kuiper.Systems.Events;
using System;
using System.Collections;
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

            if (_eventService.GameEvents != null)
            {
                foreach (var currentEvent in _eventService.GameEvents.ToList())
                {
                    if (currentEvent is SetCourseEvent)
                    {
                        _eventService.RemoveEvent(currentEvent);
                    }
                }
            }
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


        private static double GetDistanceFromSun(double x, double y)
        {
            return Math.Sqrt(Math.Pow((0 - x), 2) + Math.Pow((0 - y), 2));
        }

        public IEnumerable<CelestialBody> LookupSolarSystem(DateTime time)
        {
            var elapsedTime = _gameTimeService.GameStartDate - time;
            var destinations = new List<CelestialBody>();
            var planets = _solarSystemService.GetBodies().ToList();
            var asteroids = _solarSystemService.Asteroids;
            
            destinations.AddRange(planets);
            destinations.AddRange(asteroids);

            var sortedBodies = destinations.OrderBy(p => GetDistanceFromSun(p.GetPosition(elapsedTime).X,p.GetPosition(elapsedTime).Y));
            return sortedBodies;
        }
    }
}