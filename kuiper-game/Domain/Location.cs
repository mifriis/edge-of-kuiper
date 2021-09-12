using System;
using System.Collections.Generic;

namespace Kuiper.Domain
{
    public static class Locations
    {
        
        public static Location Luna = new Location("Luna", 384400, new List<Location>(), SatteliteType.Moon);
        public static Location Earth = new Location("Earth", 150740000, new List<Location>() { Luna }, SatteliteType.Planet);
        public static List<Location> Destinations;

        static Locations()
        {
            Destinations = new List<Location>();
            Destinations.Add(Luna);
            Destinations.Add(Earth);
        }
    }
    public class Location
    {
        public Location(string name, long orbitalRadius, List<Location> sattelites, SatteliteType type)
        {
            Name = name;
            Sattelites = sattelites;
            OrbitalRadius = orbitalRadius;
            SatteliteType = type;
        }

        public string Name { get; }
        public List<Location> Sattelites { get; }

        public long OrbitalRadius { get; }

        public SatteliteType SatteliteType { get; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public enum SatteliteType 
    {
        Planet,
        Moon,
        Asteroid
    }
}