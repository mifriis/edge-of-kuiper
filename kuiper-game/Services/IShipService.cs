using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Services
{
    public interface IShipService
    {
        IEnumerable<CelestialBody> GetPossibleDestinations();
        Ship GetShip();
        void SetShip(Ship ship);
    }
}