using System;
using Dawn;
using Kuiper.Domain;

namespace Kuiper.Services
{
    public class TimeService : ITimeService
    {
        public static DateTimeOffset RealTimeEpoch { get; private set; }
        private static readonly DateTimeOffset GameTimeEpoch = new DateTimeOffset(new DateTime(2078, 1, 8));
        private const int TickAccelerationConstant = 7; //1 real day is 7 game days

        public static void Init(DateTimeOffset realTimeEpoch)
        {
            RealTimeEpoch = realTimeEpoch;
        }
        
        [Obsolete("Use the non-static method Now() instead")]
        public static GameTime Now()
        {
            Guard.Argument(RealTimeEpoch, nameof(RealTimeEpoch))
                .NotDefault("The real time offset (start of the game) has not been set");
            
            var elapsed = DateTimeOffset.Now - RealTimeEpoch;
            return new GameTime(GameTimeEpoch + TimeSpan.FromTicks(elapsed.Ticks * TickAccelerationConstant));
        }

        public TimeSpan GetElapsedGameTime()
        {
            return Now().Value - GameTimeEpoch;
        }

        GameTime ITimeService.Now()
        {
            return Now();
        }
    }
}