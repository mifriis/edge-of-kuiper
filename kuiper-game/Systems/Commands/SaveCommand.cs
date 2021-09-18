using System;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class SaveCommand : ConsoleCommandBase
    {
        private readonly ICaptainService _captainService;
        public SaveCommand(ICaptainService captainService)
        {
            _captainService = captainService;
        }
        public override string CommandName => "Save";

        public override void Execute(string[] args)
        {
            var currentCaptain = _captainService.GetCaptain();
            currentCaptain.LastLoggedIn = GameTime.Now();
            SaveLoad.SaveGame(currentCaptain);
            ConsoleWriter.Write($"Game saved successfully.", ConsoleColor.Red);
        }
    }
}