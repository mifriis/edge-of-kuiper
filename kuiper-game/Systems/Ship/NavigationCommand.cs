using System;
using System.Collections.Generic;
using System.Linq;
using Kuiper.Services;
using Kuiper.Systems.Events;
using Humanizer;
using System.Diagnostics.CodeAnalysis;
using Kuiper.Domain.CelestialBodies;
using Terminal.Gui;
using Terminal.Gui.Graphs;

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
                var elapsedTime = _gameTimeService.GameStartDate - i;
                var solarSystem =_shipService.LookupSolarSystem(i);
                Console.Clear();
                ConsoleWriter.Write(i.Month + "-" + i.Year);

                var solarSystemSize = solarSystem.Count()*2+1;
                
                var celestialBodySpacer = 2;
                foreach (var body in solarSystem)
                {
                    var posX = body.GetPosition(elapsedTime).X;
                    var posY = body.GetPosition(elapsedTime).Y;
                    var realAngle = ((Math.Atan2(0 - posY, 0 - posX) * (180 / Math.PI)) + 360) % 360;
                    realAngle *= Math.PI / 180;
                    var normalX = (int) Math.Round(celestialBodySpacer * Math.Cos(realAngle));
                    var normalY = (int)Math.Round(celestialBodySpacer * Math.Sin(realAngle));
                    if (normalX+solarSystemSize >= 0 && normalY+solarSystemSize >= 0) //Don't render planets outside the viewport
                    {
                        ConsoleWriter.WriteAt(body.Name.Substring(0,2),(normalX+solarSystemSize)*2,normalY+solarSystemSize, body.Color);
                    }
                    celestialBodySpacer+=2;
                }
                Console.ReadKey();
            }
        }
        
        
    }
} 