// using System.Collections.Generic;
// using Kuiper.Domain.CelestialBodies;
// using System;
// using System.IO;
// using Newtonsoft.Json.Linq;
// using System.Runtime.CompilerServices;

// [assembly: InternalsVisibleTo("kuiper-tests")]
// namespace Kuiper.Repositories
// {
//     public class JsonFileSolarSystemRepository : ISolarSystemRepository
//     {
//         private List<CelestialBody> solarSystem;
//         private FileInfo jsonFile;
//         public IEnumerable<CelestialBody> GetSolarSystem()
//         {
//             if (solarSystem.Count == 0) {
//                 solarSystem = GetAllBodies(jsonFile);
//             }
//             return solarSystem;
//         }

//         public JsonFileSolarSystemRepository(FileInfo jsonFile)
//         {
//             if (!jsonFile.Exists) throw new FileNotFoundException($"Could not find {jsonFile.FullName}");
//             this.jsonFile = jsonFile;

//             solarSystem = new List<CelestialBody>();
//         }

//         private List<CelestialBody> GetAllBodies(FileInfo jsonFile) 
//         {
//             var json = ReadJsonFromFile(jsonFile);
//             var list = GetBodiesFromJson(json);

//             return list;
//         }

//         private string ReadJsonFromFile(FileInfo jsonFile)
//         {
//             return File.ReadAllText(jsonFile.FullName);
//         }

//         internal List<CelestialBody> GetBodiesFromJson(string json) 
//         {
//             var solarSystem = new List<CelestialBody>();
//             var data = JObject.Parse(json);
            
//             CreateBody(data, null, solarSystem);

//             return solarSystem;
//         }

//         private void CreateBody(JObject element, CelestialBody parent, List<CelestialBody> outputList) 
//         {
//             Enum.TryParse((string)element["type"], out CelestialBodyType bodyType);

//             var body = CelestialBody.Create((string)element["name"], (double)element["distance"], 
//                 (double)element["velocity"], (double)element["originDegrees"], parent,
//                 bodyType);
//             outputList.Add(body);

//             var satellites = (JArray)element["satellites"];
//             if (satellites == null) return;

//             foreach (var sat in satellites)
//             {
//                 CreateBody((JObject)sat, body, outputList);
//             }
//         } 
//     }
// }