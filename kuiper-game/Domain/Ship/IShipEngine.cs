namespace Kuiper.Domain.Ship
{
    public interface IShipEngine
    {
        double Cost { get; set; }
        double Mass { get; set; }
        double Thrust { get; set; }
        double SpecificImpulse { get; set; }
        double ThrustToWeightRatio { get; }
        double ExhaustVelocity { get; }
    }
}