using System.Diagnostics.CodeAnalysis;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
    public abstract class MiningBaseCommand : ConsoleCommandBase
    {
        internal readonly IMiningService _miningService;
        internal readonly IEventService _eventService;
        internal readonly IGameTimeService _gameTimeService;
        private IShipService shipService;

        public MiningBaseCommand(IMiningService miningService, IEventService eventService, IGameTimeService gameTimeService)
        {
            _miningService = miningService;
            _eventService = eventService;
            _gameTimeService = gameTimeService;
        }
        
        public override string Group => "mining";
    }
} 