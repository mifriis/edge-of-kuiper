using Kuiper.Services;

namespace Kuiper.Systems
{
    public abstract class ShipBaseCommand : ConsoleCommandBase
    {
        internal readonly IShipService _shipService;

        public ShipBaseCommand(IShipService shipService)
        {
            _shipService = shipService;
        }

        public override string Group => "ship";
    }
} 