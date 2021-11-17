using System;
using System.Linq;
using Kuiper.Services;
using Lamar;

namespace Kuiper.Systems.Events
{
    public class SetCourseEvent : IEvent
    {
        public DateTime EventTime { get; set; }
        public string EventName { get; set; }

        public void Execute(IContainer serviceLocator)
        {
            var shipService = serviceLocator.GetInstance<IShipService>();
            shipService.FinalizeJourney();
            var ship = shipService.Ship;
            ConsoleWriter.Write(EventTime.ToUniversalTime() +  " " + ship.Name + " has arrived in orbit around " + ship.CurrentLocation.Name);
        }
    }
} 