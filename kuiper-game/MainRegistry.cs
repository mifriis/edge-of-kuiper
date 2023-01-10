using System;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Kuiper.Systems.CommandInfrastructure;
using Kuiper.Systems;
using Kuiper.Repositories;
using Kuiper.Services;
using Kuiper.Systems.Events;
using System.Diagnostics.CodeAnalysis;

namespace Kuiper
{
    [ExcludeFromCodeCoverage]//Is ServiceRegistry
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
            this.AddSingleton<ISolarSystemRepository>(x => new JsonFileSolarSystemRepository(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Data/Sol.solarsystem")));
            this.AddSingleton<ITraderShipsRepository>(x => new JsonFileTraderShipsRepository(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Data/Trader.ships")));
            this.AddSingleton<ISolarSystemService, SolarSystemService>();
            this.AddSingleton<IShipService, ShipService>();
            this.AddSingleton<ICaptainService, CaptainService>();
            this.AddSingleton<ICaptainsConsole, CaptainsConsole>();
            Scan((_) =>
            {
                _.AssemblyContainingType<IEvent>();
                _.AddAllTypesOf<IEvent>(ServiceLifetime.Singleton);
            });
            this.AddSingleton<IEventService, EventService>();
            this.AddSingleton<ISaveService, SaveService>();
            this.AddSingleton<IGameTimeService, GameTimeService>();
            this.AddSingleton<IAccountService, AccountService>();
            this.AddSingleton<IMiningService, MiningService>();
            this.AddSingleton<ITraderService, TraderService>();
        }
    }
}