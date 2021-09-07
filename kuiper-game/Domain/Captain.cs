using System;
using Kuiper.Systems;
using Newtonsoft.Json;

namespace Kuiper.Domain
{
    public class Captain
    {
        [JsonConstructor]
        public Captain(string name)
        {
            Name = name;
        }

        public Captain(string name, DateTime newCaptainGameTime, DateTime newCaptainRealTime)
        {
            Name = name;
            GameLastSeen = newCaptainGameTime;
            RealLastSeen = newCaptainRealTime;
        }

        public string Name { get; }
        public DateTime GameLastSeen { get; set; }
        public DateTime RealLastSeen { get; set; }

        public Ship Ship { get; set;}

        public void MarkLastSeen()
        {
            GameLastSeen = TimeDilation.CalculateTime(GameLastSeen,RealLastSeen,DateTime.Now);
            RealLastSeen = DateTime.Now;
        }

    }
}