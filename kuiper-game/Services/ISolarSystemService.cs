using Kuiper.Domain.CelestialBodies;
using System.Collections.Generic;
using Kuiper.Domain.Mining;

namespace Kuiper.Services
{
    public interface ISolarSystemService
    {
        CelestialBody AddCelestialBody(CelestialBody celestialBody);
        void RemoveCelestialBody(CelestialBody celestialBody);
        double GetDistanceInAu(CelestialBody origin, CelestialBody destination);
        long GetDistanceInKm(CelestialBody origin, CelestialBody destination);
        IEnumerable<CelestialBody> GetBodies();
        IEnumerable<CelestialBody> GetSatellites(CelestialBody parent);
        CelestialBody GetBody(string name);
        List<CelestialBody> SolarSystem { get; set; }
        void LoadFromRepository();
        CelestialBody GetStar();
        List<Asteroid> Asteroids { get; set; }
        Asteroid AddAsteroid(Asteroid asteroid);
        void RemoveAsteroid(Asteroid asteroid);
        Asteroid GetAsteroid(string name);
    }
}