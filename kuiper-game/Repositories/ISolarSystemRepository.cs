using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Repositories
{
    public interface ISolarSystemRepository
    {
         IEnumerable<CelestialBody> GetSolarSystem();
    }
}