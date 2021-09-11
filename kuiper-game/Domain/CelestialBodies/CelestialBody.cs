using System;
using System.Collections.Generic;
using System.Drawing;

namespace kuiper.Domain.CelestialBodies
{
    public abstract class CelestialBody
    {
        public string Name => name;
        public float OrbitRadius => orbitRadius;
        public float Velocity => velocity;
        public float OriginDegrees => originDegrees;
        public PointF GetPosition(DateTime currentGameTime)
        {
            throw new NotImplementedException();
        }

        public CelestialBody Parent => parent;
        public IEnumerable<CelestialBody> Satellites => satellites;

        private string name;
        private float orbitRadius;
        private float velocity;
        private float originDegrees;
        private CelestialBody parent;
        private List<CelestialBody> satellites;

        public static CelestialBody Create(string name, float orbitRadius, float velocity, float originDegrees, CelestialBody parent, CelestialBodyType bodyType)
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

        private static CelestialBody create<T>(string name, float orbitRadius, float velocity,
            float originDegrees, CelestialBody parent) where T : CelestialBody, new()
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
    }
}