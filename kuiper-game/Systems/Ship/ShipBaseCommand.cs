using Kuiper.Services;

namespace Kuiper.Systems
{
    public abstract class ShipBaseCommand : ConsoleCommandBase
    {
        internal readonly IShipService _shipService;
        internal readonly IEventService _eventService;
        private IShipService shipService;

        public ShipBaseCommand(IShipService shipService, IEventService eventService)
        {
            _shipService = shipService;
            _eventService = eventService;
        }
        public override string Group => "ship";
    }
} 