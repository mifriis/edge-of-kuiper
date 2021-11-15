using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Services
{
    public interface IShipService
    {
        Ship Ship { get; set; }
        IEnumerable<CelestialBody> GetPossibleDestinations();
        void SetCourse(string destination);
        void FinalizeJourney();
    }
}