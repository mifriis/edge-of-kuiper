

using Kuiper.Services;

namespace Kuiper.Systems
{
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