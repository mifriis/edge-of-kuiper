using Kuiper.Domain;

namespace Kuiper.Services
{
    public static class CaptainLocator
    {
        private static Captain _captain;
        public static Captain Captain { 
            get {
                return _captain;
            }
        }

        public static void SetCaptain(Captain captain)
        {
            _captain = captain;
        }
    }
    
}