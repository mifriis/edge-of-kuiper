using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;
using System.Collections.Generic;
using System.Linq;

namespace Kuiper.Services
{
    public class ShipService : IShipService
    {
        private Ship _ship;
        private readonly ISolarSystemService _solarSystemService;

        public ShipService(ISolarSystemService solarSystemService)
        {
            _solarSystemService = solarSystemService;
        }
        
        public IEnumerable<CelestialBody> GetPossibleDestinations()
        {
            var destinations = new List<CelestialBody>();
            var planets = _solarSystemService.GetBodies().ToList();
            var moons = _solarSystemService.GetSatellites(_ship.CurrentLocation).ToList();
            destinations.AddRange(planets);
            destinations.AddRange(moons);
            return destinations;
        }

        public void SetShip(Ship ship)
        {
            _ship = ship;
        }

        public Ship GetShip()
        {
            return _ship;
        }
    }
}