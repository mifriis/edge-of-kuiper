using Kuiper.Services;
using Kuiper.Systems;
using Kuiper.Repositories;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Kuiper
{
    public class MainRegistry : ServiceRegistry
    {
        public MainRegistry()
        {
            this.AddHostedService<MainLoopWorker>();
            this.AddSingleton<ISolarSystemJsonStructureRepository>(x => new JsonFileSolarSystemRepository(new FileInfo("Data\\Sol.solarsystem")));
            this.AddSingleton<ISolarSystemRepository, JsonStringSolarSystemRepository>();
            this.AddSingleton<ISolarSystemService, SolarSystemService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
        }
    }
}