using Kuiper.Services;

namespace Kuiper.Systems
{
    public abstract class ShipBaseCommand : ConsoleCommandBase
    {
        internal readonly ICaptainService captainService;

        public ShipBaseCommand(ICaptainService captainService)
        {
            this.captainService = captainService;
        }

        public override string Group => "ship";
    }
}