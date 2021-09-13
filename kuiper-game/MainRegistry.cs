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
            this.AddSingleton<ISolarSystemRepository>(x => 
                ActivatorUtilities.CreateInstance<JsonFileSolarSystemRepository>(x, new FileInfo("Data\\Sol.solarsystem")));
            this.AddSingleton<ISolarSystemService, SolarSystemService>();
            this.AddSingleton<ICaptainService, CaptainService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
        }
    }
}