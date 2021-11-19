using Kuiper.Domain;

namespace Kuiper.Services
{
    public interface ICaptainService
    {
        Captain GetCaptain();
        Captain SetupCaptain();
        void SaveGame();
    }
}