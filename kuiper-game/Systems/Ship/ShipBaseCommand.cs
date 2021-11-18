using Kuiper.Services;

namespace Kuiper.Systems
{
    public abstract class ShipBaseCommand : ConsoleCommandBase
    {
        internal readonly IShipService _shipService;
        internal readonly IEventService _eventService;
        internal readonly IGameTimeService _gameTimeService;
        private IShipService shipService;

        public ShipBaseCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService)
        {
            _shipService = shipService;
            _eventService = eventService;
            _gameTimeService = gameTimeService;
        }
        public override string Group => "ship";
    }
} 