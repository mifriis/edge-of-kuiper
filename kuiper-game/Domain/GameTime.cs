using System;

namespace Kuiper.Domain
{
    public readonly struct GameTime
    {
        private static readonly long GameTimeEpoch = new DateTime(2078, 1, 8).Ticks;
        private const long TickAccelerationConstant = 7; //1 real day is 7 game days
        public static long RealTimeEpoch { get; private set; }
        public long Ticks { get; }

        /// <summary>
        /// Initialize a game time from a real time
        /// </summary>
        /// <param name="realTimeEpoch">The starting timestamp in real time UTC. This is equal to gametime GT 2078-01-01</param>
        /// <param name="ticks">Additional ticks</param>
        public GameTime(long realTimeEpoch, long ticks = 0)
        {
            RealTimeEpoch = realTimeEpoch;
            Ticks = ticks;
        }

        public GameTime Add(long ticks)
        {
            return new GameTime(RealTimeEpoch, ticks);
        }

        public DateTime ConvertToGameDateTime()
        {
            return new DateTime(GameTimeEpoch + Ticks);
        }

        public static GameTime Now()
        {
            if (RealTimeEpoch == 0L)
                throw new Exception("GameTime not initialized");
            
            var elapsed = DateTime.UtcNow.Ticks - RealTimeEpoch;
            return new GameTime(RealTimeEpoch, elapsed * TickAccelerationConstant);
        }
    }
}