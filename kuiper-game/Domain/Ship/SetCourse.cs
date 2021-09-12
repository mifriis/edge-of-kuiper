using System;
using System.Collections.Generic;
using Kuiper.Services;
namespace Kuiper.Domain
{
    public class CourseSet : IShipEvent
    {
        private const int chance = 80;
        public CourseSet(DateTime startTime, TimeSpan duration)
        { 
            StartTime = startTime;
            TaskDuration = duration;
        }

        public string Name => "Set course";

        public string Description => $"Lay in a course towards a new location in the solarsystem";

        public DateTime StartTime {get;}

        public TimeSpan TaskDuration {get;}

        public string StartEvent()
        {
            return $"{SolarSystemLocator.SolarSystem.Captain.Ship.Name} will arrive in orbit above {SolarSystemLocator.SolarSystem.Captain.Ship.TargetLocation.Name} on {SolarSystemLocator.SolarSystem.Captain.Ship.ArrivalTime}";
        }

        public string EndEvent()
        {
            return $"Ship arrived";
        }
    }
}