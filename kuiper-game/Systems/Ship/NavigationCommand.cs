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
            ConsoleWriter.Write("Solar System");
            var minX = -20;
            var maxX = 20;
            var minY = -20;
            var maxY = 20;
            
            var bodies = _shipService.GetPossibleDestinations().ToList()
                .Where(x => x.CelestialBodyType != CelestialBodyType.Moon)
                .Where(x => x.CelestialBodyType != CelestialBodyType.Asteroid);
            var now = _gameTimeService.ElapsedGameTime;
            var sortedBodies = bodies.OrderByDescending(p => p.GetPosition(now).Y).ThenBy(p => p.GetPosition(now).X);
            foreach (var body in sortedBodies)
            {
                var pos = body.GetPosition(now);
                var indents = "";
                for (int i = maxX; i >= pos.X; i--)
                {
                    indents += " ";
                }
                ConsoleWriter.Write((indents + body.Name.Substring(0,2)));
                
            }
            
        }
    }
} 