using Kuiper.Services;
using System.Diagnostics.CodeAnalysis;

namespace Kuiper.Systems.Trader
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.

    public abstract class BaseTraderCommand : ConsoleCommandBase
    {
        internal readonly IShipService _shipService;
        internal readonly IEventService _eventService;
        internal readonly IGameTimeService _gameTimeService;
        internal readonly ICaptainService _captainService;
        internal readonly ITraderService _traderService;

        public BaseTraderCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService, ICaptainService captainService, ITraderService traderService)
        {
            _shipService = shipService;
            _eventService = eventService;
            _gameTimeService = gameTimeService;
            _captainService = captainService;
            _traderService = traderService;
        }
        public override string Group => "trader";

    }
}
