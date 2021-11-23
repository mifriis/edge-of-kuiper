using System;
using System.Collections.Generic;
using Kuiper.Systems;
using Kuiper.Systems.Events;
using Lamar;

namespace Kuiper.Services
{
    public class GameTimeService : IGameTimeService
    {
        private static readonly DateTime gameStartDate = new DateTime(2078, 1, 1);
        private const long tickAccelerationConstant = 7; //1 real day, is 7 game days
        public TimeSpan ElapsedGameTime 
        {
            get 
            {
                var elapsedRealTime = DateTime.Now.Subtract(RealStartTime);
                var elapsedGameTime = new TimeSpan(elapsedRealTime.Ticks * tickAccelerationConstant);

                return elapsedGameTime;
            }
        }

        public DateTime GameStartDate 
        {
            get
            {
                return gameStartDate;
            }
        }

        public DateTime RealStartTime {get; set;}

        public DateTime Now()
        {
            if(RealStartTime == DateTime.MinValue)
            {
                throw new ArgumentOutOfRangeException("RealStartTime has never been set, it's required to be so either during load or captain setup");
            }

            return gameStartDate.Add(ElapsedGameTime);
        }
    }
}