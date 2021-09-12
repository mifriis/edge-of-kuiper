using System;
using System.Collections.Generic;

namespace Kuiper.Domain
{
    public class SolarSystem
    {
        public SolarSystem(DateTime now)
        {
            GameStart = now;
        }
        public DateTime GameStart { get; set;}
        
        public Captain Captain { get; set;}

        public List<Location> Locations { get; set;} = new List<Location>();
        
        
    }
}