

using System.Diagnostics.CodeAnalysis;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
    public class TimeCommand : CaptainBaseCommand
    {
        public TimeCommand(ICaptainService captainService, IGameTimeService gameTimeService) : base(captainService, gameTimeService)
        {
        }

        public override string Name => "time";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write("You look at your watch and see it is " + _gameTimeService.Now());
        }
    }
} 