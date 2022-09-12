using System;
using System.Numerics;
using Kuiper.Domain.CelestialBodies;

namespace Kuiper.Domain.Navigation;

public class NavigationBody
{
    public NavigationBody(CelestialBody celestialBody, TimeSpan elapsedTime, int renderingOrder, int solarSystemSize)
    {
        CelestialBody = celestialBody;
        var posX = celestialBody.GetPosition(elapsedTime).X;
        var posY = celestialBody.GetPosition(elapsedTime).Y;
        var realAngle = ((Math.Atan2(0 - posY, 0 - posX) * (180 / Math.PI)) + 360) % 360;
        realAngle *= Math.PI / 180;
        var normalX = (int) Math.Round(renderingOrder * Math.Cos(realAngle));
        var normalY = (int)Math.Round(renderingOrder * Math.Sin(realAngle));

        NormalisedCoordinate = new Vector2((normalX+solarSystemSize)*2, (normalY+solarSystemSize));
    }
    
    public CelestialBody CelestialBody { get; set; }
    public Vector2 NormalisedCoordinate { get; set; }
}