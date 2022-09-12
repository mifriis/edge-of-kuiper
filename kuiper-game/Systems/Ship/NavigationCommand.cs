using System;
using System.Collections.Generic;
using System.Linq;
using Kuiper.Services;
using Kuiper.Systems.Events;
using Humanizer;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Domain.Navigation;
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
            ConsoleKeyInfo input;
            Console.TreatControlCAsInput = true;
            
            var now = _gameTimeService.Now();
            var elapsedTime = _gameTimeService.GameStartDate - now;
            var solarSystem =_shipService.LookupSolarSystem(now);
            Console.Clear();
            ConsoleWriter.Write(now.ToString("d"));

            var solarSystemSize = solarSystem.Count()*2+1;
            
            var celestialBodySpacer = 2;
            var navigationBodies = new List<NavigationBody>();
            foreach (var body in solarSystem)
            {
                var navigationBody = new NavigationBody(body, elapsedTime, celestialBodySpacer, solarSystemSize);
                navigationBodies.Add(navigationBody);
                if (navigationBody.NormalisedCoordinate.X >= 0 && navigationBody.NormalisedCoordinate.Y >= 0) //Don't render planets outside the viewport
                {
                    ConsoleWriter.WriteAt(body.Name.Substring(0,2),(int)navigationBody.NormalisedCoordinate.X,(int)navigationBody.NormalisedCoordinate.Y, body.Color);
                }
                celestialBodySpacer+=2;
            }

            var currentNavBodyIndex = 0;
            do
            {
                input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.Add)
                {
                    if (currentNavBodyIndex < navigationBodies.Count()-1)
                    {
                        currentNavBodyIndex++;
                    }
                    var nav = navigationBodies[currentNavBodyIndex];
                    var infoString = BuildInfo(nav.CelestialBody);
                    ConsoleWriter.WriteInfoBox(infoString, nav.NormalisedCoordinate);
                    
                }
                if (input.Key == ConsoleKey.Subtract)
                {
                    if (currentNavBodyIndex > 0)
                    {
                        currentNavBodyIndex--;
                    }
                    
                    var nav = navigationBodies[currentNavBodyIndex];
                    var infoString = BuildInfo(nav.CelestialBody);
                    ConsoleWriter.WriteInfoBox(infoString, nav.NormalisedCoordinate);
                }
                if (input.Key == ConsoleKey.Enter)
                {
                    var nav = navigationBodies[currentNavBodyIndex];
                    var deltaVneeded = _shipService.CalculateDeltaVForJourney(nav.CelestialBody);
                    if (_shipService.Ship.CurrentLocation == nav.CelestialBody)
                    {
                        var infoString = "You are already here";
                    
                        ConsoleWriter.WriteInfoBox(infoString, nav.NormalisedCoordinate);
                    }
                    else if (_shipService.Ship.deltaV >= deltaVneeded)
                    {
                        var gameEvent = _shipService.SetCourse(nav.CelestialBody.Name);
                        var infoString = "Ship is enroute" + System.Environment.NewLine +
                                         "Arrival: " + gameEvent.EventTime.ToString("d");
                    
                        ConsoleWriter.WriteInfoBox(infoString, nav.NormalisedCoordinate);
                    }
                    else
                    {
                        var infoString = "Not enough fuel" + System.Environment.NewLine +
                                         "Needed: " + deltaVneeded + System.Environment.NewLine +
                                         "Available: " + _shipService.Ship.deltaV;
                    
                        ConsoleWriter.WriteInfoBox(infoString, nav.NormalisedCoordinate);
                    }
                    
                }
            } while (input.Key != ConsoleKey.Escape);
            Console.Clear();
            ConsoleWriter.Write("Navigation program ended. Input command.");
        }

        private string BuildInfo(CelestialBody body)
        {
            var navName = body.Name;
            var navTime = TravelTime(body);
            var navCost = TravelCost(body);
            var help = "Esc to Cancel";
            var help2 = "Enter to set course";
            var infoString = navName + System.Environment.NewLine +
                             navCost + System.Environment.NewLine +
                             navTime + System.Environment.NewLine +
                             help + System.Environment.NewLine +
                             help2;
            return infoString;
        }

        private string TravelTime(CelestialBody destination)
        {
            var travelTime = _shipService.CalculateTravelTime(destination);
            return TimeSpan.FromSeconds(travelTime.TotalSeconds).Humanize();
        }
        
        private string TravelCost(CelestialBody destination)
        {
            var deltaVNeeded = _shipService.CalculateDeltaVForJourney(destination);
            return Math.Round(deltaVNeeded / 1000, 0) + "km/s dV";
        }
        
        
    }
} 