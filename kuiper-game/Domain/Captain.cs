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

        public Captain(string name, DateTime newCaptainGameTime, DateTime newCaptainRealTime, Account account)
        {
            Name = name;
            GameLastSeen = newCaptainGameTime;
            RealLastSeen = newCaptainRealTime;
            Account = account;
        }

        public string Name { get; }
        public DateTime GameLastSeen { get; set; }
        public DateTime RealLastSeen { get; set; }

        public Ship Ship { get; set;}
        public Account Account { get; }

        public void MarkLastSeen()
        {
            GameLastSeen = TimeDilation.CalculateTime(GameLastSeen,RealLastSeen,DateTime.Now);
            RealLastSeen = DateTime.Now;
        }
    }
}