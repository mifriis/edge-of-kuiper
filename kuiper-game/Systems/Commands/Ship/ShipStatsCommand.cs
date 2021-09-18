using Kuiper.Services;

namespace Kuiper.Systems
{
    public class ShipStatsCommand : ShipBaseCommand
    {
        public ShipStatsCommand(ICaptainService captainService) : base(captainService)
        {
        }

        public override string CommandName => "stats";

        public override void Execute(string[] args)
        {
           ConsoleWriter.Write(captainService.GetCaptain().Ship.ShipStatsDescription);
        }
    }
}