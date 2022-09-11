using System;
using System.Collections.Generic;
using Kuiper.Domain;

namespace Kuiper.Services
{
    public interface ICaptainService
    {
        Captain GetCaptain();
        Captain SetupCaptain();
        void SaveGame();
        void LoadGame(string save);
        IEnumerable<string> FindSaves();
    }
}