using System.Diagnostics.CodeAnalysis;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
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