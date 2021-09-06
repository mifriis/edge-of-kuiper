using System;

namespace Kuiper.Domain
{
    public class Captain
    {
        public Captain(string name)
        {
            Name = name;

        }

        public string Name { get; }
        public DateTime LastSeen { get; set; }
    }
}