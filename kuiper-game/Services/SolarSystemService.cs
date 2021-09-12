using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;

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
            throw new System.NotImplementedException();
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
    }
}