using System;
using System.Linq;
using Kuiper.Services;
using Kuiper.Systems.Events;
using Humanizer;
using System.Diagnostics.CodeAnalysis;

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
            foreach(var destination in destinations)
            {
                ConsoleWriter.Write(counter + ") " + destination.Name);
                counter++;
            }
            var selectionInput = Console.ReadLine();
            var numericalDestination = Int32.Parse(selectionInput);
            if(numericalDestination < 1)
            {
                ConsoleWriter.Write("Selected destination not availiable.");
            }
            var chosenDestination = destinations[numericalDestination-1];
            var gameEvent = _shipService.SetCourse(chosenDestination.Name);
            
            ConsoleWriter.Write(_shipService.Ship.Name + " is " + _shipService.Ship.Status + " " + _shipService.Ship.TargetLocation.Name + " and will arrive on " + gameEvent.EventTime);
        }
    }
} 