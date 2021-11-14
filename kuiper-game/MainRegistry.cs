using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Kuiper.Systems.CommandInfrastructure;
using Kuiper.Systems;

namespace Kuiper
{
    public class MainRegistry : ServiceRegistry
    {
        public MainRegistry()
        {
            this.AddHostedService<MainLoopWorker>();
            this.AddSingleton<ICommandParser, ConsoleCommandParser>();
            Scan((_) =>
            {
                _.AssemblyContainingType<IConsoleCommand>();
                _.AddAllTypesOf<IConsoleCommand>(ServiceLifetime.Singleton);
            });
            // this.AddSingleton<ISolarSystemRepository>(x => new JsonFileSolarSystemRepository(new FileInfo("Data/Sol.solarsystem")));
            // this.AddSingleton<ISolarSystemService, SolarSystemService>();
            // this.AddSingleton<ICaptainService, CaptainService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
        }
    }
}