using Kuiper.Domain;

namespace Kuiper.Services
{
    public static class SolarSystemLocator
    {
        private static SolarSystem _solarSystem;
        public static SolarSystem SolarSystem { 
            get {
                return _solarSystem;
            }
        }

        public static void SetSolarSystem(SolarSystem solarSystem)
        {
            _solarSystem = solarSystem;
        }
    }
    
}