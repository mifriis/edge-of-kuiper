

using System.Diagnostics.CodeAnalysis;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
    public class SaveCommand : CaptainBaseCommand
    {
        public SaveCommand(ICaptainService captainService, IGameTimeService gameTimeService) : base(captainService, gameTimeService)
        {
        }

        public override string Name => "save";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write("Saving game...", System.ConsoleColor.Red);
            _captainService.SaveGame();
            ConsoleWriter.Write("Game saved", System.ConsoleColor.Red);
        }
    }
} 