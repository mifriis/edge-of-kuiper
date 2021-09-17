using System;
using Kuiper.Domain;

namespace Kuiper.Services
{
    public interface ITimeService
    {
        GameTime Now();
        TimeSpan GetElapsedGameTime();
    }
}