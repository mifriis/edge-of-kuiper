

using Kuiper.Services;

namespace Kuiper.Systems
{
    public abstract class CaptainBaseCommand : ConsoleCommandBase
    {
        internal readonly ICaptainService _captainService;

        public CaptainBaseCommand(ICaptainService captainService)
        {
            _captainService = captainService;
        }

        public override string Group => "captain";
    }
} 