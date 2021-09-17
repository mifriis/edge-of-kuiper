namespace Kuiper.Domain.CelestialBodies
{
    public class Moon :CelestialBody
    {
        public Moon(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent) : base(name, orbitRadius, velocity, originDegrees, parent)
        {
        }
    }
}