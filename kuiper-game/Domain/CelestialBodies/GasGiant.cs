namespace Kuiper.Domain.CelestialBodies
{
    public class GasGiant :CelestialBody
    {
        public GasGiant(string name, double orbitRadius, double velocity, double originDegrees, CelestialBody parent) : base(name, orbitRadius, velocity, originDegrees, parent)
        {
        }
    }
}