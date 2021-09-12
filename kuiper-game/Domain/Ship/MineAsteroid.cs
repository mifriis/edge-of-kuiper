using System;
using System.Collections.Generic;
using Kuiper.Services;
namespace Kuiper.Domain
{
    public class MineAsteroid : IShipEvent
    {
        private const int chance = 80;
        public MineAsteroid(DateTime startTime, TimeSpan duration)
        { 
            StartTime = startTime;
            TaskDuration = duration;
        }

        public string Name => "Asteroid Mining";

        public string Description => $"Mine the asteroid you are orbiting for ore and minerals";

        public DateTime StartTime {get;}

        public TimeSpan TaskDuration {get;}

        public string StartEvent()
        {
            return $"You begin mining the asteroid and hope to have a full cargohold in {TaskDuration.TotalHours} hours";
        }

        public string EndEvent()
        {
            var minerals = 0;
            var time = TaskDuration.TotalHours;
            do
            {
                time--;
                if(DiceFactory.Roller().D100(60,0))
                {
                    minerals++;
                }
            } while (time >= 0);
            return $"After mining for {TaskDuration.TotalHours}, you have filled your cargohold with {minerals} tons of minerals";
        }
    }
}