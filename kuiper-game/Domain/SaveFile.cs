using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Domain.Mining;
using Kuiper.Systems.Events;
using Kuiper.Domain.Ship;

namespace Kuiper.Domain
{
    public class SaveFile
    {
        public Captain Captain { get; set;}
        public Ship.Ship Ship { get; set;}
        public List<IEvent> GameEvents { get; set; }
        public List<CelestialBody> SolarSystem { get; set; }
        public List<Asteroid> Asteroids { get; set; }
    }
}