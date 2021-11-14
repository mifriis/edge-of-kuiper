using Kuiper.Services;

namespace Kuiper.Systems
{
    public class LocationCommand : ShipBaseCommand
    {
        public LocationCommand(IShipService shipService) : base(shipService)
        {
        }

        public override string Name => "location";

        public override void Execute(string[] args)
        {
            
            ConsoleWriter.Write("Your ship is currently at " + _shipService.GetShip().CurrentLocation.Name);
        }
    }
} 