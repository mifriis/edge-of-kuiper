using Kuiper.Domain.CelestialBodies;
using System;
using System.Numerics;
using System.Collections.Generic;
using Kuiper.Domain;

namespace Kuiper.Services
{
    public interface ISolarSystemService
    {
        SolarSystem GetSolarSystem();
        SolarSystem SetupGame();
        double GetDistanceInAu(CelestialBody origin, CelestialBody destination);
        long GetDistanceInKm(CelestialBody origin, CelestialBody destination);
        IEnumerable<CelestialBody> GetBodies(CelestialBodyType type);
        IEnumerable<CelestialBody> GetSatellites(CelestialBody parent);
        IEnumerable<CelestialBody> GetNearestBodies(int count);
        CelestialBody GetBody(string name);
        IEnumerable<CelestialBody> GetBodies();
    }
}