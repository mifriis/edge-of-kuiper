namespace Kuiper.Domain.CelestialBodies
{
    public class Star :CelestialBody
    {
        public Star(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent) : base(name, orbitRadius, velocity, originDegrees, parent)
        {
        }
    }
}