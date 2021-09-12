using Kuiper.Domain.CelestialBodies;
using System;

namespace Kuiper.Services
{
    public interface ISolarSystemService
    {
         public double GetDistanceInAu(CelestialBody origin, CelestialBody destination) {
             throw new NotImplementedException();
         }
    }
}