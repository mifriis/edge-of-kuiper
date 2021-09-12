using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kuiper.Domain
{
    public static class Locations
    {
        
        public static Location Luna = new Location("Luna", 384400, new List<Location>(), SatelliteType.Moon);
        public static Location Earth = new Location("Earth", 150740000, new List<Location>() { Luna }, SatelliteType.Planet);
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
        public Location(string name, long orbitalRadius, List<Location> satellites, SatelliteType satelliteType)
        {
            Name = name;
            Satellites = satellites;
            OrbitalRadius = orbitalRadius;
            SatelliteType = satelliteType;
        }

        public string Name { get; }
        public List<Location> Satellites { get; }

        public long OrbitalRadius { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SatelliteType SatelliteType { get; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public enum SatelliteType 
    {
        Planet,
        Moon,
        Asteroid
    }
}