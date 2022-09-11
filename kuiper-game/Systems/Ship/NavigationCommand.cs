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

                var solX = 19;
                var solY = 19;
                origRow = Console.CursorTop;
                origCol = Console.CursorLeft;
                
                WriteAt("S",solX*2,solY,"DarkYellow");

                var counter = 2;
                foreach (var body in solarSystem)
                {
                    if (body.CelestialBodyType != CelestialBodyType.Star)
                    {
                        var posX = body.GetPosition(elapsedTime).X;
                        var posY = body.GetPosition(elapsedTime).Y;
                        var realAngle = ((Math.Atan2(0 - posY, 0 - posX) * (180 / Math.PI)) + 360) % 360;
                        realAngle *= Math.PI / 180;
                        var cos = Math.Cos(realAngle);
                        var sin = Math.Sin(realAngle);
                        var fakePoint = new Point((int)Math.Round(counter * cos), (int)Math.Round(counter * sin));
                        if (fakePoint.X+solX >= 0 && fakePoint.Y+solY >= 0)
                        {
                            WriteAt(body.Name.Substring(0,1),(fakePoint.X+solX)*2,fakePoint.Y+solY, body.Color);
                        }
                        counter+=2;
                    }
                }
                Console.ReadKey();
            }
        }
        
        int origRow;
        int origCol;
        private void WriteAt(string s, int x, int y, string color)
        {
            try
            {
                Console.SetCursorPosition(origCol+x, origRow+y);
                ConsoleWriter.Write(s,color);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
    }
} 