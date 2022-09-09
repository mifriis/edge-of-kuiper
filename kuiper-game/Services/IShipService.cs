using System;
using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Domain.Ship;
using Kuiper.Systems.Events;

namespace Kuiper.Services
{
    public interface IShipService
    {
        Ship Ship { get; set; }
        IEnumerable<CelestialBody> GetPossibleDestinations();
        SetCourseEvent SetCourse(string destination);
        void FinalizeJourney(double deltaVSpent);
        TimeSpan CalculateTravelTime(CelestialBody destination);
        double CalculateDeltaVForJourney(CelestialBody destination);
        String LookupSolarSystem(DateTime time);
    }
}