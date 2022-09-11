using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kuiper.Domain.CelestialBodies
{
    public class CelestialBody
    {
        private const int AUINKM =  149597871; // 1 AU in KM. Maybe this belongs somewhere else?
        public string Name { get; set; }

        public double OrbitRadius { get; set; } 

        public double Velocity { get; set; }

        public double OriginDegrees { get; set; }
        
        public string Color { get; set; }

        public Vector2 GetPosition(TimeSpan gametimePassed)
        {
            if (Parent == null) return new Vector2(0,0);
            var velocityInRadiansPerSecond = 
                Velocity / (OrbitRadius * AUINKM);
            var positionInRadians = OriginDegrees * (Math.PI/180);
            positionInRadians += (velocityInRadiansPerSecond * gametimePassed.TotalSeconds);

            var x = Parent.GetPosition(gametimePassed).X + Math.Cos(positionInRadians) * OrbitRadius;
            var y = Parent.GetPosition(gametimePassed).Y + Math.Sin(positionInRadians) * OrbitRadius;

            return new Vector2((float)x, (float)y);
        }

        public CelestialBody Parent { get; set; }

        public List<CelestialBody> Satellites { get; set; }

        public static CelestialBody Create(string name, double orbitRadius, 
            CelestialBody parent, CelestialBodyType bodyType) 
        {
            Random rnd = new Random();

            var originDegrees = rnd.NextDouble() * 360;
            return Create(name, orbitRadius, originDegrees, parent, bodyType);
        }

        public static CelestialBody Create(string name, double orbitRadius, double originDegrees, 
            CelestialBody parent, CelestialBodyType bodyType) 
        {
            const double VELOCITYCONSTANT = 29.8;
            var velocity = VELOCITYCONSTANT / (orbitRadius / Math.Sqrt(orbitRadius));

            return Create(name, orbitRadius, velocity, originDegrees, parent, bodyType, "White");
        }

        public static CelestialBody Create(string name, double orbitRadius, double velocity, 
            double originDegrees, CelestialBody parent, CelestialBodyType bodyType, string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                color = "White";
            }
            var body = new CelestialBody
            {
                Name = name,
                OrbitRadius = orbitRadius,
                Velocity = velocity,
                OriginDegrees = originDegrees,
                Satellites = new List<CelestialBody>(),
                Parent = parent,
                CelestialBodyType = bodyType,
                Color = color
            };

            if (body.Parent != null && body.CelestialBodyType != CelestialBodyType.Asteroid) //yeeeeeahhh...
            {
                parent.AddSatellite(body);
            }
            return body;
        }

        public CelestialBodyType CelestialBodyType { get; set; }

        public void AddSatellite(CelestialBody satellite)
        {
            Satellites.Add(satellite);
        }
    }
}