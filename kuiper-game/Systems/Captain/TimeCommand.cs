

using Kuiper.Services;

namespace Kuiper.Systems
{
    public class TimeCommand : CaptainBaseCommand
    {
        public TimeCommand(ICaptainService captainService) : base(captainService)
        {
        }

        public override string Name => "time";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write("You look at your watch and see it is " + GameTime.Now());
        }
    }
} 