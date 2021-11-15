using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;
using System.Collections.Generic;
using System.Linq;

namespace Kuiper.Services
{
    public class ShipService : IShipService
    {
 
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

        public Ship Ship { get; set; }
    }
}