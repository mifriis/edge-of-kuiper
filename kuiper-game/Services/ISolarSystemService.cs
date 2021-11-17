using Kuiper.Domain.CelestialBodies;
using System.Collections.Generic;

namespace Kuiper.Services
{
    public interface ISolarSystemService
    {
        double GetDistanceInAu(CelestialBody origin, CelestialBody destination);
        long GetDistanceInKm(CelestialBody origin, CelestialBody destination);
        IEnumerable<CelestialBody> GetBodies();
        IEnumerable<CelestialBody> GetSatellites(CelestialBody parent);
        CelestialBody GetBody(string name);
        List<CelestialBody> SolarSystem { get; set; }
        void LoadFromRepository();
    }
}