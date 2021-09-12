using Kuiper.Domain;

namespace Kuiper.Services
{
    public interface ISolarSystemService
    {
        SolarSystem GetSolarSystem();
        SolarSystem SetupGame();
    }
}