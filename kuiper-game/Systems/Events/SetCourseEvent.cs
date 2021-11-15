using System;
using System.Linq;
using Kuiper.Services;

namespace Kuiper.Systems.Events
{
    public class SetCourseEvent : IEvent
    {
        IShipService _shipService;
        public SetCourseEvent(IShipService shipService)
        {
            _shipService = shipService;
        }
        public DateTime EventTime { get; set; }
        public string EventName { get; set; }

        public void Execute(string[] args)
        {
            _shipService.FinalizeJourney();
            var ship = _shipService.Ship;
            ConsoleWriter.Write(EventTime.ToUniversalTime() +  " " + ship.Name + " has arrived in orbit around " + ship.CurrentLocation.Name);
        }
    }
} 