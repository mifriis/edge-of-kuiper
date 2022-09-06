using System.Collections.Generic;
using Kuiper.Domain.Mining;
using Kuiper.Systems.Events;

namespace Kuiper.Services;

public interface IMiningService
{
    public IEnumerable<Asteroid> ScannedAsteroids();
    public ScanForAsteroidsEvent ScanForAsteroids();
}