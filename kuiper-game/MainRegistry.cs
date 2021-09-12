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
            this.AddSingleton<ISolarSystemService, SolarSystemService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
        }
    }
}