using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kuiper.Services
{
    public class ShipService : IShipService
    {
        public Ship Ship { get; set; }
        private readonly ISolarSystemService _solarSystemService;

        public ShipService(ISolarSystemService solarSystemService)
        {
            _solarSystemService = solarSystemService;
        }
        
        public IEnumerable<CelestialBody> GetPossibleDestinations()
        {
            var destinations = new List<CelestialBody>();
            var planets = _solarSystemService.GetBodies().ToList();
            var moons = _solarSystemService.GetSatellites(Ship.CurrentLocation).ToList();
            destinations.AddRange(planets);
            destinations.AddRange(moons);
            return destinations;
        }

        public void SetCourse(string destination)
        {
            var celestialBody = _solarSystemService.GetBody(destination);
            if(celestialBody == null)
            {
                return;
            }
            if(GetPossibleDestinations().ToList().Contains(celestialBody))
            {
                Ship.TargetLocation = celestialBody;
                Ship.Status = ShipStatus.Enroute;
            }
        }

        public void FinalizeJourney()
        {
            Ship.CurrentLocation = Ship.TargetLocation;
            Ship.TargetLocation = null;
            Ship.Status = Domain.ShipStatus.InOrbit;
        }

        public TimeSpan CalculateTravelTime(CelestialBody destination)
        {
            var distance = _solarSystemService.GetDistanceInKm(Ship.CurrentLocation, destination);
            var hours = distance / Ship.Speed;
            var timespan = TimeSpan.FromHours(hours);
            return timespan;
        }

        
    }
}