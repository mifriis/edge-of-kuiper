using System;
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

        public Captain(string name, DateTime startTime, Account account)
        {
            Name = name;
            StartTime = startTime;
            Account = account;
        }

        public string Name { get; }
        public DateTime StartTime { get; set; }
        public DateTime LastLoggedIn { get; set;}

        public Ship Ship { get; set;}
        public Account Account { get; }
    }
}