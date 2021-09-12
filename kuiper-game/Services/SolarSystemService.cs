using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System.Linq;

namespace Kuiper.Services
{
    public class SolarSystemService : ISolarSystemService
    {
        public IEnumerable<CelestialBody> GetBodies(CelestialBodyType type)
        {
            throw new System.NotImplementedException();
        }

        public CelestialBody GetBody(string name)
        {
            return SolarSystem.Where(b => string.Equals(name, b.Name, System.StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }

        public double GetDistanceInAu(CelestialBody origin, CelestialBody destination)
        {
            throw new System.NotImplementedException();
        }

        public long GetDistanceInKm(CelestialBody origin, CelestialBody destination)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CelestialBody> GetNearestBodies(int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CelestialBody> GetSatellites(CelestialBody parent)
        {
            throw new System.NotImplementedException();
        }

        private List<CelestialBody> SolarSystem;
        public SolarSystemService(ISolarSystemRepository repository)
        {
            SolarSystem = new List<CelestialBody>();
            SolarSystem = CreateSolarSystem(repository).ToList();
        }

        private IEnumerable<CelestialBody> CreateSolarSystem(ISolarSystemRepository repository) {
            return repository.GetSolarSystem();
        }
    }
}