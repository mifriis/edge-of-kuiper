using System;
using System.Linq;
using Kuiper.Services;
using Kuiper.Systems.Events;

namespace Kuiper.Systems
{
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
            _shipService.SetCourse(chosenDestination.Name);
            var travelTime = _shipService.CalculateTravelTime(chosenDestination);
            _eventService.AddEvent(new SetCourseEvent() { EventTime = _gameTimeService.Now().Add(travelTime), EventName = "Travel Event" });
            ConsoleWriter.Write(_shipService.Ship.Name + " is " + _shipService.Ship.Status + " " + _shipService.Ship.TargetLocation.Name);
            
        }
    }
} 