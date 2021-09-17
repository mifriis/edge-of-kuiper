namespace Kuiper.Domain.CelestialBodies
{
    public class Asteroid :CelestialBody
    {
        public Asteroid(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent) : base(name, orbitRadius, velocity, originDegrees, parent)
        {
        }
    }
}