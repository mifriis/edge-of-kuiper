using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Systems;
using Kuiper.Systems.Events;
using Newtonsoft.Json;

namespace Kuiper.Services
{
    [ExcludeFromCodeCoverage] //Heavily using statics for IO stuff.
    public class SaveService: ISaveService
    {
        private readonly string savePath = Directory.CreateDirectory(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "saves")).FullName;
        public void Save(SaveFile saveFile)
        {
            var json = JsonConvert.SerializeObject(saveFile, Formatting.Indented, 
                new JsonSerializerSettings 
                { 
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.Auto

                });
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(savePath, saveFile.Captain.Name +".save")))
            {
                    outputFile.WriteLine(json);
            }
        }

        public IEnumerable<string> LookForSaves(string captain)
        {
            var filePaths = Directory.GetFiles(savePath).ToList().Where(file => file.Contains(".save"));
            var files = filePaths.Select(file => file.Split(Path.DirectorySeparatorChar).LastOrDefault());
            if(captain == string.Empty) 
            {
                return files;
            }
            files = files.Where(file => file.Equals($"{captain}.save"));
            return files; 
        }
        
        public IEnumerable<string> LookForSaves()
        {
            var filePaths = Directory.GetFiles(savePath).ToList().Where(file => file.Contains(".save"));
            var files = filePaths.Select(file => file.Split(Path.DirectorySeparatorChar).LastOrDefault());
            return files; 
        }

        public SaveFile Load(string captainName)
        {
            var save = System.IO.File.ReadAllText(Path.Combine(savePath,captainName));
            var saveFile = JsonConvert.DeserializeObject<SaveFile>(save, 
            new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto
            });
            return saveFile;
        }
    }
}