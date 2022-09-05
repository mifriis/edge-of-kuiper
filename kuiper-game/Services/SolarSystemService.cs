using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System.Linq;
using System.Numerics;
using System;
using System.ComponentModel.DataAnnotations;

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
        
        public CelestialBody GetStar()
        {
            var star = SolarSystem.SingleOrDefault(b => b.CelestialBodyType == CelestialBodyType.Star);
            return star;
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

        public CelestialBody AddCelestialBody(CelestialBody celestialBody)
        {
            if (GetBody(celestialBody.Name) != null)
            {
                throw new ArgumentException("Celestial body already exists. A unique name is required");
            }    
            SolarSystem.Add(celestialBody);
            celestialBody = GetBody(celestialBody.Name);
            return celestialBody;
        }
        
        public void RemoveCelestialBody(CelestialBody celestialBody)
        {
            if (GetBody(celestialBody.Name) == null)
            {
                return; //Doesn't exist, all good?
            }
            celestialBody = GetBody(celestialBody.Name);
            if (celestialBody.Satellites.Count > 0)
            {
                //No orhans
                foreach (var satellite in celestialBody.Satellites)
                {
                    SolarSystem.Remove(satellite);
                }
            }
            SolarSystem.Remove(celestialBody);
        }

        
        public SolarSystemService(ISolarSystemRepository repository, IGameTimeService gameTimeService)
        {
            _repository = repository;
            _gameTimeService = gameTimeService;
        }

    }
}