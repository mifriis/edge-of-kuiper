using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System.Collections.Generic;

namespace Kuiper.Tests.FakeRepositories
{
    //For Fake 
    public class FakeSolarSystemRepository : ISolarSystemRepository
    {
        private List<CelestialBody> celestialBodies;

        public FakeSolarSystemRepository()
        {
            var bodySun = CelestialBody.Create("Sun", 0, null, CelestialBodyType.Star);

            celestialBodies = new List<CelestialBody>()
            {
                CelestialBody.Create("Merkury", 20, bodySun, CelestialBodyType.Planet),
                CelestialBody.Create("Earth", 50, bodySun, CelestialBodyType.Planet),
                CelestialBody.Create("Jupiter", 150, bodySun, CelestialBodyType.GasGiant)
            };

            celestialBodies.Add(bodySun);
        }

        public IEnumerable<CelestialBody> GetSolarSystem()
        {
            return celestialBodies;
        }
    }
}
