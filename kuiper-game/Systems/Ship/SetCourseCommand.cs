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
    public class SetCourseCommand : ShipBaseCommand
    {
        public SetCourseCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService) : base(shipService, eventService, gameTimeService)
        {
        }

        public override string Name => "setCourse";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write("Select destination:");
            var destinations = _shipService.GetPossibleDestinations().ToList();
            var counter = 1;
            var impossibleDestinations = new List<CelestialBody>();
            var possibleDestinations = new List<CelestialBody>();
            foreach(var destination in destinations)
            {
                var deltaVNeeded = _shipService.CalculateDeltaVForJourney(destination);
                var shipDvBudget = _shipService.Ship.deltaV;
                if(deltaVNeeded > shipDvBudget)
                {
                    impossibleDestinations.Add(destination);
                }
                else
                {
                    possibleDestinations.Add(destination);
                }
            }

            foreach (var destination in impossibleDestinations)
            {
                ConsoleWriter.Write(destination.Name + " - Lack of fuel ", ConsoleColor.DarkYellow);
            }
            
            foreach (var destination in possibleDestinations)
            {
                var deltaVNeeded = _shipService.CalculateDeltaVForJourney(destination);
                var travelTime = _shipService.CalculateTravelTime(destination);
                ConsoleWriter.Write(counter + ") " + destination.Name + " in " + TimeSpan.FromSeconds(travelTime.TotalSeconds).Humanize() + ", costing " + Math.Round(deltaVNeeded/1000,0) + "km/s dV");
                counter++;
            }
            
            var selectionInput = Console.ReadLine();
            var numericalDestination = Int32.Parse(selectionInput);
            if(numericalDestination < 1)
            {
                ConsoleWriter.Write("Selected destination not available.");
            }
            var chosenDestination = possibleDestinations[numericalDestination-1];
            var gameEvent = _shipService.SetCourse(chosenDestination.Name);
            
            ConsoleWriter.Write(_shipService.Ship.Name + " is " + _shipService.Ship.Status + " " + _shipService.Ship.TargetLocation.Name + " and will arrive on " + gameEvent.EventTime);
        }
    }
} 