using System;
using System.Collections.Generic;
using System.Linq;
using Kuiper.Services;
using Kuiper.Systems.Events;
using Humanizer;
using System.Diagnostics.CodeAnalysis;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
    public class NavigationCommand : ShipBaseCommand
    {
        public NavigationCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService) : base(shipService, eventService, gameTimeService)
        {
        }

        public override string Name => "navigation";

        public override void Execute(string[] args)
        {
            var now = _gameTimeService.Now();
                
            for (DateTime i = now; i < now.AddYears(100); i = i.AddMonths(1))
            {
                var solarSystem =_shipService.LookupSolarSystem(i);
                Console.Clear();
                ConsoleWriter.Write(i.Month + "-" + i.Year);
                ConsoleWriter.Write(solarSystem);
                //System.Threading.Thread.Sleep(100);
                Console.ReadKey();
            }
             // var solarSystem =_shipService.LookupSolarSystem(now);
             // ConsoleWriter.Write(solarSystem);
        }
    }
} 