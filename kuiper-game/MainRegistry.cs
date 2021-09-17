using System;
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
            this.AddSingleton<ISolarSystemRepository>(x => new JsonFileSolarSystemRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Sol.solarsystem.json")));
            this.AddSingleton<ISolarSystemService, SolarSystemService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
        }
    }
}