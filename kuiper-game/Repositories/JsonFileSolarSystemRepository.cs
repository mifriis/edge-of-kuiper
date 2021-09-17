using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("kuiper-tests")]
namespace Kuiper.Repositories
{
    public class JsonFileSolarSystemRepository : ISolarSystemRepository
    {
        private readonly string _filePath;

        private static readonly Dictionary<string, Type> _typeMap = new()
        {
            {"Planet", typeof(Planet)},
            {"Star", typeof(Star)},
            {"DwarfPlanet", typeof(DwarfPlanet)},
            {"GasGiant", typeof(GasGiant)},
            {"Moon", typeof(Moon)},
            {"Asteroid", typeof(Asteroid)},
        };

        private IEnumerable<CelestialBody> _solarSystem;
        private IEnumerable<CelestialBody> SolarSystem
        {
            get { return _solarSystem ??= LoadFromDisk(); }
        }
        public IEnumerable<CelestialBody> GetSolarSystem()
        {
            return SolarSystem;
        }

        public JsonFileSolarSystemRepository(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Could not find {Path.GetFileName(filePath)}");
            _filePath = filePath;
        }

        private IEnumerable<CelestialBody> LoadFromDisk()
        {
            var json = File.ReadAllText(_filePath);
            
            var rawSystem = JArray.Parse(json);
            var bodies = new List<CelestialBody>();

            while (rawSystem.Count > 0)
            {
                var candidates = rawSystem
                    .Where(x => 
                        !string.IsNullOrEmpty(x.Value<string>("parent")) || 
                        !bodies.Any(b => b.Name == x.Value<string>("parent")))
                    .ToList();
                
                foreach (var token in candidates)
                {
                    bodies.Add(CreateInstance(token, bodies));
                    rawSystem.Remove(token);
                }
            }

            return bodies;
        }

        private CelestialBody CreateInstance(JToken token, IEnumerable<CelestialBody> system)
        {
            var type = _typeMap[token.Value<string>("type")];

            var parentCandidate = system.SingleOrDefault(x => x.Name == token.Value<string>("parent"));

            return CelestialBody.Create(
                type, 
                token.Value<string>("name"), 
                token.Value<double>("orbitRadius"),
                token.Value<double>("velocity"),
                token.Value<double>("originDegrees"),
                parentCandidate);
        }
    }
}