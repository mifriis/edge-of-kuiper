

using System.Diagnostics.CodeAnalysis;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
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