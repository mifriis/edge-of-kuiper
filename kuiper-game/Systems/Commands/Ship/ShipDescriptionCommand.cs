using Kuiper.Services;

namespace Kuiper.Systems
{
    public class ShipDescriptionCommand : ShipBaseCommand
    {
        public ShipDescriptionCommand(ICaptainService captainService) : base(captainService)
        {
        }

        public override string CommandName => "description";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write(captainService.GetCaptain().Ship.Description);
        }
    }
}