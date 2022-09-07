using System;

namespace Kuiper.Domain.Ship
{
    public class FuelTank : IShipModule
    {
        public FuelTank(ModuleSize size)
        {
            Size = size;
        }
        public ModuleSize Size { get; }

        public ModuleType Type 
        {
            get 
            {
                return ModuleType.FuelTank;
            }    
        }

        public double Mass
        {
            get
            {
                //smallest about 1 ton of mass, bigger tanks are more efficient 
                return ((int)Size/2) * 1;
            }
        }

        public double FuelMass
        {
            get
            {
                return (int)Size * Convert.ToDouble(6.25);
            }
        }
    }
}