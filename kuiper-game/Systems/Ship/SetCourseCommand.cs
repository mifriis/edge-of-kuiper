using System;
using System.Linq;
using Kuiper.Services;
using Kuiper.Systems.Events;

namespace Kuiper.Systems
{
    public class SetCourseCommand : ShipBaseCommand
    {
        public SetCourseCommand(IShipService shipService, IEventService eventService) : base(shipService, eventService)
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
            _eventService.AddEvent(new SetCourseEvent() { EventTime = GameTime.Now(), EventName = "Travel Event" });
            ConsoleWriter.Write(_shipService.Ship.Name + " is " + _shipService.Ship.Status + " " + _shipService.Ship.TargetLocation.Name);
            
        }
    }
} 