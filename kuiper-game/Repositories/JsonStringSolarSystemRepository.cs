using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kuiper.Domain.CelestialBodies;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("kuiper-tests")]
namespace Kuiper.Repositories
{
    public class JsonStringSolarSystemRepository :ISolarSystemRepository
    {
        private readonly ISolarSystemJsonStructureRepository jsonStructureRepository;

        public JsonStringSolarSystemRepository(ISolarSystemJsonStructureRepository repository)
        {
            jsonStructureRepository = repository;
        }

        public IEnumerable<CelestialBody> GetSolarSystem()
        {
            var list = GetBodiesFromJson(jsonStructureRepository.GetSolarSystemJsonStructure());
            return list;
        }

        internal List<CelestialBody> GetBodiesFromJson(string json)
        {
            var solarSystem = new List<CelestialBody>();
            var data = JObject.Parse(json);
            CreateBody(data, null, solarSystem);
            return solarSystem;
        }

        private void CreateBody(JObject element, CelestialBody parent, List<CelestialBody> outputList) 
        {
            

            Enum.TryParse((string)element["type"], out CelestialBodyType bodyType);

            var body = CelestialBody.Create((string)element["name"], (double)element["distance"], 
                (double)element["velocity"], (double)element["originDegrees"], parent,
                bodyType);
            outputList.Add(body);

            var satellites = (JArray)element["satellites"];
            if (satellites == null) return;

            foreach (var sat in satellites)
            {
                CreateBody((JObject)sat, body, outputList);
            }
        }
    }
}