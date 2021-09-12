using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Kuiper.Domain;
using Newtonsoft.Json;

namespace Kuiper.Systems
{
    public static class SaveLoad
    {
        private static string savePath = Directory.CreateDirectory(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "saves")).FullName;
        private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects};
        public static void SaveGame(Captain captain) 
        {
            var captainJson = JsonConvert.SerializeObject(captain, settings);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(savePath, captain.Name +".save")))
            {
                    outputFile.WriteLine(captainJson);
            }
        }

        public static IEnumerable<string> LookForSaves(string captain)
        {
            var filePaths = Directory.GetFiles(savePath).ToList().Where(file => file.Contains(".save"));
            var files = filePaths.Select(file => file.Split("\\").LastOrDefault());
            if(captain == string.Empty) 
            {
                return files;
            }
            files = files.Where(file => file.Equals($"{captain}.save"));
            return files; 
        }

        public static Captain Load(string saveGame)
        {
            var save = System.IO.File.ReadAllText(Path.Combine(savePath,saveGame));
            var captain = JsonConvert.DeserializeObject<Captain>(save, settings);
            return captain;
        }
    }
}