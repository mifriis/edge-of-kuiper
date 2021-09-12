using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public class SolarSystemService : ISolarSystemService
    {
        private SolarSystem _currentSystem;
        public SolarSystem SetupGame()
        {
            if(_currentSystem == null) 
            {
                ConsoleWriter.Write("Greetings captain, what is your name?");
                var name = Console.ReadLine();
                var saves = SaveLoad.LookForSaves(name);
                if(saves.Count() > 0)
                {
                    _currentSystem = SaveLoad.Load(saves.FirstOrDefault());
                    GameTime.RealStartTime = _currentSystem.GameStart;
                    SolarSystemLocator.SetSolarSystem(_currentSystem);
                    ConsoleWriter.Write($"Welcome back Captain {_currentSystem.Captain.Name}, you were last seen on {_currentSystem.Captain.LastLoggedIn}!");    
                    _currentSystem.Captain.Ship.StatusReport();
                    return _currentSystem;
                }
                
                _currentSystem = new SolarSystem(DateTime.Now);
                _currentSystem.Captain = new Captain(name);
                GameTime.RealStartTime = _currentSystem.GameStart;
                SolarSystemLocator.SetSolarSystem(_currentSystem);
                _currentSystem.Captain.Ship = new Ship("Bullrun","Sloop", 40000);
                _currentSystem.Captain.Ship.CurrentLocation = Locations.Earth;
                _currentSystem.Captain.Ship.Status = ShipStatus.InOrbit;
                ConsoleWriter.Write($"Welcome, Captain {name}, you have logged in on {GameTime.Now()}");
                return _currentSystem;
            }
            return _currentSystem;
        }
        public SolarSystem GetSolarSystem()
        {
            if(_currentSystem != null)
            {
                return _currentSystem;
            }
            throw new NullReferenceException("Solar system is not yet setup or loaded");
        }
    }
}
