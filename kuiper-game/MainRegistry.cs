using Kuiper.Services;
using Kuiper.Systems;
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
            this.AddSingleton<IRandom, Random>();
            this.AddSingleton<IDiceRoller, DiceRoller>();
        }
    }
}