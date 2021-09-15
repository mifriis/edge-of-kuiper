using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kuiper.Domain.CelestialBodies
{
    public abstract class CelestialBody
    {
        private const int AUINKM =  149597871; // 1 AU in KM. Maybe this belongs somewhere else?
        public string Name
        {
            get
            {
                return name;
            }
        }

        public double OrbitRadius
        {
            get
            {
                return orbitRadius;
            }
        }

        public double Velocity
        {
            get
            {
                return velocity;
            }
        }

        public double OriginDegrees
        {
            get
            {
                return originDegrees;
            }
        }

        public Vector2 GetPosition(TimeSpan gametimePassed)
        {
            if (parent == null) return new Vector2(0,0);
            var velocityInRadiansPerSecond = 
                velocity / (OrbitRadius * AUINKM);
            var positionInRadians = originDegrees * (Math.PI/180);
            positionInRadians += (velocityInRadiansPerSecond * gametimePassed.TotalSeconds);

            var x = parent.GetPosition(gametimePassed).X + Math.Cos(positionInRadians) * OrbitRadius;
            var y = parent.GetPosition(gametimePassed).Y + Math.Sin(positionInRadians) * OrbitRadius;

            return new Vector2((float)x, (float)y);
        }

        public CelestialBody Parent
        {
            get
            {
                return parent;
            }
        }

        public IEnumerable<CelestialBody> Satellites
        {
            get
            {
                return satellites;
            }
        }

        private string name;
        private double orbitRadius;
        private double velocity;
        private double originDegrees;
        private CelestialBody parent;
        private List<CelestialBody> satellites;

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

            return Create(name, orbitRadius, velocity, originDegrees, parent, bodyType);
        }

        public static CelestialBody Create(string name, double orbitRadius, double velocity, 
            double originDegrees, CelestialBody parent, CelestialBodyType bodyType)
        {
            CelestialBody body = null;

            switch (bodyType)
            {
                case CelestialBodyType.Star:
                    body = create<Star>(name, orbitRadius, velocity, originDegrees, null);
                    break;
                case CelestialBodyType.Planet:
                    body = create<Planet>(name, orbitRadius, velocity, originDegrees, parent);
                    break;
                case CelestialBodyType.Moon:
                    body = create<Moon>(name, orbitRadius, velocity, originDegrees, parent);
                    break;
                case CelestialBodyType.GasGiant:
                    body = create<GasGiant>(name, orbitRadius, velocity, originDegrees, parent);
                    break;
                case CelestialBodyType.DwarfPlanet:
                    body = create<DwarfPlanet>(name, orbitRadius, velocity, originDegrees, parent);
                    break;
                case CelestialBodyType.Asteroid:
                    body = create<Asteroid>(name, orbitRadius, velocity, originDegrees, parent);
                    break;
            }

            if (body.parent != null)
            {
                parent.AddSatellite(body);
            }
            return body;
        }

        public void AddSatellite(CelestialBody satellite)
        {
            satellites.Add(satellite);
        }

        private static CelestialBody create<T>(string name, double orbitRadius, double velocity,
            double originDegrees, CelestialBody parent) where T : CelestialBody, new()
        {
            var body = new T()
            {
                name = name,
                orbitRadius = orbitRadius,
                velocity = velocity,
                originDegrees = originDegrees,
                satellites = new List<CelestialBody>(),
                parent = parent
            };

            return body;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}