using System;
using System.Linq;
using Kuiper.Services;

namespace Kuiper.Systems
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
            var ship = _shipService.Ship;
            ship.CurrentLocation = ship.TargetLocation;
            ship.TargetLocation = null;
            ship.Status = Domain.ShipStatus.InOrbit;
            ConsoleWriter.Write(EventTime.ToUniversalTime() +  " " + ship.Name + " has arrived in orbit around " + ship.CurrentLocation.Name);
        }
    }
} 