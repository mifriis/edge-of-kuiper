using System;
using System.Diagnostics.CodeAnalysis;

namespace Kuiper.Services
{
    [ExcludeFromCodeCoverage]
    public class Random : IRandom
    {
        public int Next(int start, int end)
        {
            var seed = new Random().Next(start,end);
            return seed;
        }
    }

}