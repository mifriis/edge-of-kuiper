using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public class CaptainService : ICaptainService
    {
        private readonly ISolarSystemService _solarSystemService;
        public CaptainService(ISolarSystemService solarSystemService)
        {
            _solarSystemService = solarSystemService;
        }
        private Captain _currentCaptain;
        

        public Captain SetupCaptain()
        {
            if(_currentCaptain == null) 
            {
                ConsoleWriter.Write("Greetings captain, what is your name?");
                var name = Console.ReadLine();             
                _currentCaptain = new Captain(name, DateTime.Now);
                GameTime.RealStartTime = _currentCaptain.StartTime;
                _currentCaptain.Ship = new Ship("Bullrun","Sloop", 40000);
                _currentCaptain.Ship.CurrentLocation = _solarSystemService.GetBody("Earth");
                ConsoleWriter.Write($"Welcome, Captain {name}, you have logged in on {GameTime.Now()}");
                return _currentCaptain;
            }
            return _currentCaptain;
        }

        public Captain GetCaptain()
        {
            if(_currentCaptain != null)
            {
                return _currentCaptain;
            }
            throw new NullReferenceException("Captain is not yet setup or loaded");
        }
    }
}
