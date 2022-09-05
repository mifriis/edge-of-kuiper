using System;
using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Domain.Mining;

public class Asteroid : CelestialBody
{
    public AsteroidType AsteroidType { get; set; }
    public AsteroidSize AsteroidSize { get; set; }
    public int Yield { get; set; }

    public Asteroid(Double orbitRadius, CelestialBody parent, AsteroidType type, AsteroidSize size, int yield)
    {
        AsteroidType = type;
        AsteroidSize = size;
        Yield = yield;
        Name = GenerateName();
        var body = Create(Name, orbitRadius, parent, CelestialBodyType.Asteroid);
        OrbitRadius = body.OrbitRadius;
        OriginDegrees = body.OriginDegrees;
        Parent = parent;
        CelestialBodyType = body.CelestialBodyType;
        Velocity = body.Velocity;
        Satellites = new List<CelestialBody>();
    }

    private String GenerateName()
    {
        var id = new Random().Next(100000, 900000);
        return AsteroidType + "-" + AsteroidSize + "" + id;
    }
}