

namespace Kuiper.Systems
{
    public abstract class CaptainBaseCommand : ConsoleCommandBase
    {
        // internal readonly ICaptainService captainService;

        public CaptainBaseCommand()//ICaptainService captainService)
        {
            // this.captainService = captainService;
        }

        public override string Group => "captain";
    }
} 