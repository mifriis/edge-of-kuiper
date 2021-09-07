using System;

namespace Kuiper.Systems
{
    public static class TimeDilation
    {
        public static DateTime GameStartDate = new DateTime(2078,1,1);
        private const long tickAccelerationConstant = 7; //1 real day, is 7 game days
        public static DateTime CalculateTime(DateTime gameLastPlayerDate, DateTime realLastSeen, DateTime realNow)
        {   
            var realTimeSpan = realNow.Subtract(realLastSeen);
            var acceleratedTimeSpan = new TimeSpan(realTimeSpan.Ticks * tickAccelerationConstant);
            
            var newGameDate = gameLastPlayerDate.Add(acceleratedTimeSpan);
            
            return newGameDate;
        }       
    }
}