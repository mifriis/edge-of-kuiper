using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Kuiper.Domain.Ship;

[assembly: InternalsVisibleTo("kuiper-tests")]
namespace Kuiper.Repositories
{
    public class JsonFileTraderShipsRepository : ITraderShipsRepository
    {
        private List<Ship> Ships;
        private FileInfo jsonFile;
        public IEnumerable<Ship> GetTraderShips()
        {
            if (Ships.Count == 0)
            {
                Ships = GetAllBodies(jsonFile);
            }
            return Ships;
        }

        public JsonFileTraderShipsRepository(FileInfo jsonFile)
        {
            if (!jsonFile.Exists) throw new FileNotFoundException($"Could not find {jsonFile.FullName}");
            this.jsonFile = jsonFile;

            Ships = new List<Ship>();
        }

        private List<Ship> GetAllBodies(FileInfo jsonFile)
        {
            var json = ReadJsonFromFile(jsonFile);
            var list = GetBodiesFromJson(json);

            return list;
        }

        private string ReadJsonFromFile(FileInfo jsonFile)
        {
            return File.ReadAllText(jsonFile.FullName);
        }

        internal List<Ship> GetBodiesFromJson(string json)
        {
            var ships = new List<Ship>();
            var data = JObject.Parse(json);

            CreateBody(data, ships);

            return ships;
        }

        private void CreateBody(JObject element, List<Ship> outputList)
        {
            var shipArray = (JArray)element["ships"];
            if (shipArray == null) return;

            foreach (var ship in shipArray)
            {
                var body = new Ship((string)ship["Name"], new ShipEngine(cost: (double)ship["Engine"]["Cost"], mass: (double)ship["Engine"]["Mass"], thrust: (double)ship["Engine"]["Thrust"], specificImpulse: (double)ship["Engine"]["SpecificImpulse"]), (double)ship["ShipMass"]);
                outputList.Add(body);
            }
        }
    }
}