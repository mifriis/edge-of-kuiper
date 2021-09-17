using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System.Linq;
using System.Numerics;
using System;
using Kuiper.Domain;

namespace Kuiper.Services
{
    public class SolarSystemService : ISolarSystemService
    {
        private readonly ITimeService _timeService;
        private const int AUINKM =  149597871; // 1 AU in KM. Maybe this belongs somewhere else?
        
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

            var originPosition = origin.GetPosition(_timeService.GetElapsedGameTime());
            var destinationPosition = destination.GetPosition(_timeService.GetElapsedGameTime());

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
            throw new System.NotImplementedException();
        }

        private List<CelestialBody> SolarSystem;
        public SolarSystemService(ISolarSystemRepository repository, ITimeService timeService)
        {
            _timeService = timeService;
            SolarSystem = new List<CelestialBody>();
            SolarSystem = CreateSolarSystem(repository).ToList();
        }

        private IEnumerable<CelestialBody> CreateSolarSystem(ISolarSystemRepository repository) {
            return repository.GetSolarSystem();
        }
    }
}