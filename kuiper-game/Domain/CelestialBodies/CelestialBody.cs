using System;
using System.Collections.Generic;
using System.Numerics;
using LamarCodeGeneration.Util;

namespace Kuiper.Domain.CelestialBodies
{
    public abstract class CelestialBody
    {
        private const int AUINKM = 149597871; // 1 AU in KM. Maybe this belongs somewhere else?
        public string Name { get; private set; }

        public double OrbitRadius { get; private set; }

        public double Velocity { get; private set; }

        public double OriginDegrees { get; private set; }

        public CelestialBody Parent { get; private set; }

        protected CelestialBody(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent)
        {
            Name = name;
            OrbitRadius = orbitRadius;
            Velocity = velocity;
            OriginDegrees = originDegrees;
            Parent = parent;
        }
        
        public Vector2 GetPosition(TimeSpan gametimePassed)
        {
            if (Parent == null) return new Vector2(0, 0);
            var velocityInRadiansPerSecond =
                Velocity / (OrbitRadius * AUINKM);
            var positionInRadians = OriginDegrees * (Math.PI / 180);
            positionInRadians += (velocityInRadiansPerSecond * gametimePassed.TotalSeconds);

            var x = Parent.GetPosition(gametimePassed).X + Math.Cos(positionInRadians) * OrbitRadius;
            var y = Parent.GetPosition(gametimePassed).Y + Math.Sin(positionInRadians) * OrbitRadius;

            return new Vector2((float) x, (float) y);
        }

        public static T Create<T>(string name, double orbitRadius, CelestialBody parent)
            where T : CelestialBody
        {
            Random rnd = new Random();

            var originDegrees = rnd.NextDouble() * 360;
            return Create<T>(name, orbitRadius, originDegrees, parent);
        }

        public static T Create<T>(string name, double orbitRadius, double originDegrees, CelestialBody parent)
            where T : CelestialBody
        {
            const double VELOCITYCONSTANT = 29.8;
            var velocity = VELOCITYCONSTANT / (orbitRadius / Math.Sqrt(orbitRadius));

            return Create<T>(name, orbitRadius, velocity, originDegrees, parent);
        }

        public static T Create<T>(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent)
            where T : CelestialBody
        {
            return (T)Create(typeof(T), name, orbitRadius, velocity, originDegrees, parent);
        }
        
        public static CelestialBody Create(Type celestialType, string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent)
        {

            var body = (CelestialBody)Activator.CreateInstance(celestialType,
                name, orbitRadius, velocity, originDegrees, parent);
            
            return body;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}