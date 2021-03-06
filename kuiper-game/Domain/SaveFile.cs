using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Systems.Events;

namespace Kuiper.Domain
{
    public class SaveFile
    {
        public Captain Captain { get; set;}
        public Ship Ship { get; set;}
        public List<IEvent> GameEvents { get; set; }
        public List<CelestialBody> SolarSystem { get; set; }
    }
}