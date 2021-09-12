using System;
using System.Collections.Generic;

namespace Kuiper.Domain
{
    public class SolarSystem
    {
        private DateTime now;

        public SolarSystem(DateTime now)
        {
            this.now = now;
        }

        public List<Location> Locations { get; set;} = new List<Location>();
        public Captain Captain { get; set;}
        public DateTime GameStart { get; set;}
    }
}