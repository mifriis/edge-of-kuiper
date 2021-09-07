using System.Collections.Generic;

namespace Kuiper.Domain
{
    public static class Locations
    {
        
        public static Location Luna = new Location("Luna", 384400, new List<Location>());
        public static Location Earth = new Location("Earth", 150740000, new List<Location>() { Luna });
        public static IEnumerable<Location> Destinations = new List<Location>() { Earth, Luna};
    }
    public class Location
    {
        public Location(string name, long orbitalRadius, IEnumerable<Location> sattelites)
        {
            Name = name;
            Sattelites = sattelites;
            OrbitalRadius = orbitalRadius;
        }

        public string Name { get; }
        public IEnumerable<Location> Sattelites { get; }

        public long OrbitalRadius { get; }
    }
}