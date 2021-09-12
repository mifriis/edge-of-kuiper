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

        public string Name { get; }
        public DateTime LastLoggedIn { get; set;}

        public Ship Ship { get; set;}
    }
}