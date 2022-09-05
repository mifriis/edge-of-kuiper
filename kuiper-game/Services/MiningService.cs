using System;
using System.Collections.Generic;
using Kuiper.Domain.Mining;
using Kuiper.Systems.Events;

namespace Kuiper.Services;

public class MiningService : IMiningService
{
    private readonly ISolarSystemService _solarSystemService;
    private readonly IEventService _eventService;
    private readonly IGameTimeService _gameTimeService;
    
    public List<Asteroid> Asteroids { get; set; }

    public MiningService(ISolarSystemService solarSystemService, IEventService eventService, IGameTimeService gameTimeService)
    {
        _solarSystemService = solarSystemService;
        _eventService = eventService;
        _gameTimeService = gameTimeService;

        if (Asteroids == null)
        {
            Asteroids = new List<Asteroid>();
        }
    }

    public IEnumerable<Asteroid> ScannedAsteroids()
    {
        return Asteroids;
    }

    public ScanForAsteroidsEvent ScanForAsteroids()
    {
        var finishTime = _gameTimeService.Now().Add(TimeSpan.FromDays(1));

        var gameEvent = new ScanForAsteroidsEvent() {EventTime = finishTime, EventName = "Asteroid Scanning"};
        _eventService.AddEvent(gameEvent);
        return gameEvent;
    }

    public void DeleteAsteroid(Asteroid asteroid)
    {
        Asteroids.Remove(asteroid);
    }

    public void AddAsteroid(Asteroid asteroid)
    {
        Asteroids.Add(asteroid);
    }
}