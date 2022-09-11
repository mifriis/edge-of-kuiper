using System;
using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Systems;
using Kuiper.Systems.Events;

namespace Kuiper.Services
{
    public interface ISaveService
    {
        void Save(SaveFile saveFile);
        SaveFile Load(string captainName);
        IEnumerable<string> LookForSaves(string captain);
        IEnumerable<string> LookForSaves();
    }
}