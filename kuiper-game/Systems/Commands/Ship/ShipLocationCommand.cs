using Kuiper.Services;

namespace Kuiper.Systems
{
    public class ShipLocationCommand : ShipBaseCommand
    {
        public ShipLocationCommand(ICaptainService captainService) : base(captainService)
        {
        }

        public override string CommandName => "location";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write(captainService.GetCaptain().Ship.LocationDescription);
        }
    }
}