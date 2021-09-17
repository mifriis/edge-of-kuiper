namespace Kuiper.Domain.CelestialBodies
{
    public class Planet :CelestialBody
    {
        public Planet(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent) : base(name, orbitRadius, velocity, originDegrees, parent)
        {
        }
    }
}