using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace Kuiper
{
    public class MainRegistry : ServiceRegistry
    {
        public MainRegistry()
        {
            this.AddHostedService<MainLoopWorker>();
        }
    }
}