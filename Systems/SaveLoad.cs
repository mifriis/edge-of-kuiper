using System;
using System.IO;
using System.Reflection;
using Kuiper.Domain;
using Newtonsoft.Json;

namespace Kuiper.Systems
{
    public class SaveLoad
    {
        public SaveLoad()
        {
            
        }

        public void SaveGame(Captain captain) 
        {
            var captainJson = JsonConvert.SerializeObject(captain);
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); 
            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(path,captain.Name +".json")))
            {
                    outputFile.WriteLine(captainJson);
            }
        }
    }
}