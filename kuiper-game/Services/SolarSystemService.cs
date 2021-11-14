using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System.Linq;
using System.Numerics;
using System;

namespace Kuiper.Services
{
    public class SolarSystemService : ISolarSystemService
    {
        private const int AUINKM =  149597871; // 1 AU in KM. Maybe this belongs somewhere else?

        public IEnumerable<CelestialBody> GetBodies(CelestialBodyType type)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CelestialBody> GetBodies()
        {
            var star = SolarSystem.SingleOrDefault(b => b is Star);
            return star.Satellites;
        } 

        public CelestialBody GetBody(string name)
        {
            return SolarSystem.Where(b => string.Equals(name, b.Name, System.StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }

        public double GetDistanceInAu(CelestialBody origin, CelestialBody destination)
        {

            var originPosition = origin.GetPosition(GameTime.ElapsedGameTime);
            var destinationPosition = destination.GetPosition(GameTime.ElapsedGameTime);

            return Vector2.Distance(originPosition, destinationPosition);
        }

        public long GetDistanceInKm(CelestialBody origin, CelestialBody destination)
        {
            return Convert.ToInt64(GetDistanceInAu(origin, destination) * AUINKM);
        }

        public IEnumerable<CelestialBody> GetNearestBodies(int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CelestialBody> GetSatellites(CelestialBody parent)
        {
            return parent.Satellites;
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