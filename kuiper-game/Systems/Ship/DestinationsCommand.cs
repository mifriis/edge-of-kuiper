using System;
using Humanizer;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class DestinationsCommand : ShipBaseCommand
    {
        public DestinationsCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService) : base(shipService, eventService, gameTimeService)
        {
        }

        public override string Name => "destinations";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write("From here you can set a course towards these destinations");
            var destinations = _shipService.GetPossibleDestinations();
            foreach(var destination in destinations)
            {
                var deltaVNeeded = _shipService.CalculateDeltaVForJourney(destination);
                var travelTime = _shipService.CalculateTravelTime(destination);
                var shipDvBudget = _shipService.Ship.deltaV;
                if(deltaVNeeded > shipDvBudget)
                {
                    ConsoleWriter.Write(destination.Name + " in " + TimeSpan.FromSeconds(travelTime.TotalSeconds).Humanize() + ", costing " + Math.Round(deltaVNeeded/1000,0) + "km/s dV", ConsoleColor.DarkYellow);
                }
                else
                {
                    ConsoleWriter.Write(destination.Name + " in " + TimeSpan.FromSeconds(travelTime.TotalSeconds).Humanize() + ", costing " + Math.Round(deltaVNeeded/1000,0) + "km/s dV");
                }

            }
            
        }
    }
} 