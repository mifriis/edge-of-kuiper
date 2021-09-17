namespace Kuiper.Domain.CelestialBodies
{
    public class DwarfPlanet :CelestialBody
    {
        public DwarfPlanet(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent) : base(name, orbitRadius, velocity, originDegrees, parent)
        {
        }
    }
}