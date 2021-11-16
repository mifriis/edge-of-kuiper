using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Services;

namespace Kuiper.Systems
{

    public class ShipSetCourseCommand : ShipBaseCommand
    {
        public ShipSetCourseCommand(ICaptainService captainService) : base(captainService)
        {
        }

        public override string Name => "set-course";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write($"What location should the ship set a course for?");
            foreach (var location in Locations.Destinations)
            {
                ConsoleWriter.Write($"* {location.Name}");
            }
            var input = Console.ReadLine();
            var target = Locations.Destinations.First(x => x.Name == input);
            if (target != null)
            {
                var courseText = captainService.SetCourse(target);
                ConsoleWriter.Write(courseText);
                return;
            }
            ConsoleWriter.Write($"No location found with the name {target}");
        }
    }
}