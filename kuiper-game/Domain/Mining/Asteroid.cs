using System;
using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Domain.Mining;

public class Asteroid : CelestialBody
{
    public AsteroidType AsteroidType { get; set; }
    public AsteroidSize AsteroidSize { get; set; }
    public int Yield { get; set; }
    
    public Asteroid(AsteroidType type, AsteroidSize size, int yield, Double orbitRadius, Double originDegrees, Double velocity, CelestialBody parent)
    {
        AsteroidType = type;
        AsteroidSize = size;
        Yield = yield;
        Name = GenerateName();
        OrbitRadius = orbitRadius;
        OriginDegrees = originDegrees;
        Parent = parent;
        Velocity = velocity;
        CelestialBodyType = CelestialBodyType.Asteroid;
    }

    private String GenerateName()
    {
        var id = new Random().Next(100000, 900000);
        return AsteroidType + "-" + AsteroidSize + "" + id;
    }
}