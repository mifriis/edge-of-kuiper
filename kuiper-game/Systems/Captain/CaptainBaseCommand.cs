

using Kuiper.Services;

namespace Kuiper.Systems
{
    public abstract class CaptainBaseCommand : ConsoleCommandBase
    {
        internal readonly ICaptainService _captainService;
        internal readonly IGameTimeService _gameTimeService;

        public CaptainBaseCommand(ICaptainService captainService, IGameTimeService gameTimeService)
        {
            _captainService = captainService;
            _gameTimeService = gameTimeService;
        }

        public override string Group => "captain";
    }
} 