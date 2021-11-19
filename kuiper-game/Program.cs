using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kuiper
{
    [ExcludeFromCodeCoverage]//Startup
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLamar(new MainRegistry())
                .UseConsoleLifetime();
        
    }
}
