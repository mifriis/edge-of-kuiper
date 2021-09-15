using System;
using System.Collections.Generic;
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
        
        public Captain Captain { get; set;}

        public ISolarSystemService SolarSystemService { get; set;}
        
        
    }
}