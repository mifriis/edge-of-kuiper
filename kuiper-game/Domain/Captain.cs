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

        public Captain(string name, GameTime startTime)
        {
            Name = name;
            StartTime = startTime;
        }

        public string Name { get; }
        public GameTime StartTime { get; set; }
        public GameTime LastLoggedIn { get; set;}

        public Ship Ship { get; set;}
    }
}