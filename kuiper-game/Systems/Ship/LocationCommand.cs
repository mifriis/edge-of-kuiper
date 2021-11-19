using Kuiper.Services;

namespace Kuiper.Systems
{
    public class LocationCommand : ShipBaseCommand
    {
        public LocationCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService) : base(shipService, eventService, gameTimeService)
        {
        }

        public override string Name => "location";

        public override void Execute(string[] args)
        {
            
            ConsoleWriter.Write("Your ship is currently at " + _shipService.Ship.CurrentLocation.Name);
        }
    }
} 