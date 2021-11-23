using System.Diagnostics.CodeAnalysis;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
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