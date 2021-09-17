using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("kuiper-tests")]
namespace Kuiper.Repositories
{
    public class JsonFileSolarSystemRepository :ISolarSystemJsonStructureRepository
    {
        private FileInfo jsonFile;

        public JsonFileSolarSystemRepository(FileInfo jsonFile)
        {
            if (!jsonFile.Exists) throw new FileNotFoundException($"Could not find {jsonFile.FullName}");
            this.jsonFile = jsonFile;
        }

        public string GetSolarSystemJsonStructure()
        {
            return ReadJsonFromFile(jsonFile);
        }

        private string ReadJsonFromFile(FileInfo jsonFile)
        {
            return File.ReadAllText(jsonFile.FullName);
        }
    }
}