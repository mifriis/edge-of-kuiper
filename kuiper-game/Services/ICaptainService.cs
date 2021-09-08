using Kuiper.Domain;

namespace Kuiper.Services
{
    public interface ICaptainService
    {
        string SetCourse(Location targetLocation);
        Captain SetupCaptain();
    }
}