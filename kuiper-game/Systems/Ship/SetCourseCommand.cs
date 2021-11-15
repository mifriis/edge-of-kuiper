using System;
using System.Linq;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class SetCourseCommand : ShipBaseCommand
    {
        public SetCourseCommand(IShipService shipService) : base(shipService)
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
            ConsoleWriter.Write(_shipService.Ship.Name + " is " + _shipService.Ship.Status + " " + _shipService.Ship.TargetLocation.Name);
            
        }
    }
} 