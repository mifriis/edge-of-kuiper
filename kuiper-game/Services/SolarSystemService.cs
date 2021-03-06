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
        public List<CelestialBody> SolarSystem { get; set; }

        private ISolarSystemRepository _repository;
        private readonly IGameTimeService _gameTimeService;

        public IEnumerable<CelestialBody> GetBodies()
        {
            var star = SolarSystem.SingleOrDefault(b => b.CelestialBodyType == CelestialBodyType.Star);
            return star.Satellites;
        } 

        public CelestialBody GetBody(string name)
        {
            return SolarSystem.Where(b => string.Equals(name, b.Name, System.StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }

        public double GetDistanceInAu(CelestialBody origin, CelestialBody destination)
        {

            var originPosition = origin.GetPosition(_gameTimeService.ElapsedGameTime);
            var destinationPosition = destination.GetPosition(_gameTimeService.ElapsedGameTime);

            return Vector2.Distance(originPosition, destinationPosition);
        }

        public long GetDistanceInKm(CelestialBody origin, CelestialBody destination)
        {
            return Convert.ToInt64(GetDistanceInAu(origin, destination) * AUINKM);
        }

        public IEnumerable<CelestialBody> GetSatellites(CelestialBody parent)
        {
            return parent.Satellites;
        }

        public void LoadFromRepository()
        {
            SolarSystem = _repository.GetSolarSystem().ToList();
        }

        
        public SolarSystemService(ISolarSystemRepository repository, IGameTimeService gameTimeService)
        {
            _repository = repository;
            _gameTimeService = gameTimeService;
        }

    }
}