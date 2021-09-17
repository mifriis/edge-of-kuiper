using System;
using System.Globalization;
using Dawn;
using Kuiper.Services;

namespace Kuiper.Domain
{
    public readonly struct GameTime
    {
        public DateTimeOffset Value { get; }

        /// <summary>
        /// The game time after genesis GT 2078-1-8
        /// </summary>
        /// <param name="value">The time in GameTime</param>
        public GameTime(DateTimeOffset value)
        {
            Guard.Argument(value, nameof(value)).Min(new DateTimeOffset(new DateTime(2078, 1, 8)));

            Value = value;
        }

        public GameTime Add(TimeSpan time)
        {
            return new GameTime(Value.Add(time));
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static bool operator < (GameTime left, GameTime right)
        {
            return left.Value < right.Value;
        }

        public static bool operator >(GameTime left, GameTime right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Shorthand to get Elapsed game time based on real time played
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use ITimeService.Now() instead")]
        public static GameTime Now()
        {
            return TimeService.Now();
        }
    }
}