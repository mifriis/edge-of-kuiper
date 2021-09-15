using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [Command("ship")]
    public class ShipConsoleCommand : IConsoleCommand
    {
        private readonly ICaptainService captainService;

        public ShipConsoleCommand(ICaptainService captainService)
        {
            this.captainService = captainService;
        }

        [Command("set-course")]
        public void SetCourse()
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

        [Command("description")]
        public void GetDescription()
        {
            ConsoleWriter.Write(captainService.GetCaptain().Ship.Description);
        }

        [Command("stats")]
        public void GetStats()
        {
            ConsoleWriter.Write(captainService.GetCaptain().Ship.ShipStatsDescription);
        }

        [Command("location")]
        public void GetLocation()
        {
            ConsoleWriter.Write(captainService.GetCaptain().Ship.LocationDescription);
        }
    }
}