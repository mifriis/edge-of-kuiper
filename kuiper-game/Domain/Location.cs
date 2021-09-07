using System.Collections.Generic;

namespace Kuiper.Domain
{
    public static class Locations
    {
        public static Location Earth = new Location("Earth", 150740000, new List<Location>() { Luna });
        public static Location Luna = new Location("Luna", 384400, null);
    }
    public class Location
    {
        public Location(string name, long orbitalRadius, IEnumerable<Location> sattelites)
        {
            Name = name;
            Sattelites = sattelites;
            orbitalRadius = OrbitalRadius;
        }

        public string Name { get; }
        public IEnumerable<Location> Sattelites { get; }

        public long OrbitalRadius { get; }
    }
}