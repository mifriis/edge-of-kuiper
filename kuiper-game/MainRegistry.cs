using Kuiper.Services;
using Kuiper.Systems;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Kuiper
{
    public class MainRegistry : ServiceRegistry
    {
        public MainRegistry()
        {
            this.AddHostedService<MainLoopWorker>();
            this.AddSingleton<ICaptainService, CaptainService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
            Scan((_) =>
            {
                _.AssemblyContainingType<ICustomConsoleCommand>();
                _.AddAllTypesOf<ICustomConsoleCommand>(ServiceLifetime.Singleton);
            });
        }
    }
}