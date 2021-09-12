using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public class CaptainService : ICaptainService
    {
        private Captain _currentCaptain;
        public Captain SetupCaptain()
        {
            if(_currentCaptain == null) 
            {
                ConsoleWriter.Write("Greetings captain, what is your name?");
                var name = Console.ReadLine();
                var saves = SaveLoad.LookForSaves(name);
                if(saves.Count() > 0)
                {
                    _currentCaptain = SaveLoad.Load(saves.FirstOrDefault());
                    GameTime.RealStartTime = _currentCaptain.StartTime;
                    CaptainLocator.SetCaptain(_currentCaptain);
                    ConsoleWriter.Write($"Welcome back Captain {_currentCaptain.Name}, you were last seen on {_currentCaptain.LastLoggedIn}!");    
                    _currentCaptain.Ship.StatusReport();
                    return _currentCaptain;
                }
                
                _currentCaptain = new Captain(name, DateTime.Now);
                GameTime.RealStartTime = _currentCaptain.StartTime;
                CaptainLocator.SetCaptain(_currentCaptain);
                _currentCaptain.Ship = new Ship("Bullrun","Sloop", 40000);
                _currentCaptain.Ship.CurrentLocation = Locations.Earth;
                _currentCaptain.Ship.Status = ShipStatus.InOrbit;
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
