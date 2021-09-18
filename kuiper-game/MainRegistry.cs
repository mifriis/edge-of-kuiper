using Kuiper.Services;
using Kuiper.Systems;
using Kuiper.Systems.CommandInfrastructure;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace Kuiper
{
    public class MainRegistry : ServiceRegistry
    {
        public MainRegistry()
        {
            this.AddHostedService<MainLoopWorker>();
            this.AddSingleton<ICaptainService, CaptainService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
            this.AddSingleton<ICommandParser, ConsoleCommandParser>();
            Scan((_) =>
            {
                _.AssemblyContainingType<IConsoleCommand>();
                _.AddAllTypesOf<IConsoleCommand>(ServiceLifetime.Singleton);
            });
        }
    }
}