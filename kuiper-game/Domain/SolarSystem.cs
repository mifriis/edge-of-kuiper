using System;
using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Services;

namespace Kuiper.Domain
{
    public class SolarSystem
    {
        public SolarSystem(DateTime now)
        {
            GameStart = now;
        }
        public DateTime GameStart { get; set;}

        public List<CelestialBody> CelestialBodies { get; set;}
        
        public Captain Captain { get; set;}

        public ISolarSystemService SolarSystemService { get; set;}
        
        
    }
}