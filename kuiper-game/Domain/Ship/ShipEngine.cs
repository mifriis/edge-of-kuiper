namespace Kuiper.Domain.Ship
{
    public class ShipEngine : IShipEngine
    {
        public ShipEngine(double cost, double mass, double thrust, double specificImpulse)
        {
            Cost = cost;
            Mass = mass;
            Thrust = thrust;
            SpecificImpulse = specificImpulse;
        }
        public double Cost { get; set; } //Credits
        public double Mass { get; set; } //Tons
        public double Thrust { get; set; } //Newtons
        public double SpecificImpulse { get; set; } //ISP

        public double ThrustToWeightRatio
        {
            get
            {
                var twr = (Thrust * 1000) / ((Mass * 1000) * Physics.STANDARD_GRAVITY);
                return twr;
            }
        }

        public double ExhaustVelocity
        {
            get
            {
                var vex = SpecificImpulse * Physics.STANDARD_GRAVITY;
                return vex;
            }
        }
    }
}