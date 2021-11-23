namespace Kuiper.Domain
{
    public interface IShipEngine
    {
        double Cost { get; }
        double Mass { get; }
        double Thrust { get; } 
        double SpecificImpulse { get; }
        double ThrustToWeightRatio { get; }
        double ExhaustVelocity { get; }
    }
}