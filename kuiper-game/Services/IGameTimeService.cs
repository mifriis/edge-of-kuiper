using System;
using System.Collections.Generic;
using Kuiper.Systems;
using Kuiper.Systems.Events;

namespace Kuiper.Services
{
    public interface IGameTimeService
    {
        DateTime Now();
        TimeSpan ElapsedGameTime { get; }
        DateTime GameStartDate { get; }
        DateTime RealStartTime {get; set;}
        

        
    }
}