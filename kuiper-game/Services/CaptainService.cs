using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public class CaptainService : ICaptainService
    {
        private readonly ISolarSystemService _solarSystemService;
        private readonly IEventService _eventService;
        private readonly IShipService _shipService;
        private readonly ISaveService _saveService;

        public CaptainService(ISolarSystemService solarSystemService, IShipService shipService, IEventService eventService, ISaveService saveService)
        {
            _solarSystemService = solarSystemService;
            _eventService = eventService;
            _shipService = shipService;
            _saveService = saveService;
        }
        private Captain _currentCaptain;
        

        public Captain SetupCaptain()
        {
            if(_currentCaptain == null) 
            {
                ConsoleWriter.Write("Greetings captain, what is your name?");
                var name = Console.ReadLine();
                var saves = _saveService.LookForSaves(name);
                if(saves.Count() > 0)
                {
                    LoadGame(saves.FirstOrDefault());
                    ConsoleWriter.Write($"Welcome back Captain {_currentCaptain.Name}, you were last seen on {_currentCaptain.LastLoggedIn}!");    
                    return _currentCaptain;
                }             
                _currentCaptain = new Captain(name, DateTime.Now);
                _solarSystemService.LoadFromRepository();
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

        public void SaveGame()
        {
            var saveFile = new SaveFile();
            saveFile.SolarSystem = _solarSystemService.SolarSystem;
            saveFile.GameEvents = _eventService.GameEvents;                        
            saveFile.Ship = _shipService.Ship;
            saveFile.Captain = _currentCaptain;
            _saveService.Save(saveFile);
        }

        public void LoadGame(string save)
        {
            var saveFile = _saveService.Load(save);
            _solarSystemService.SolarSystem = saveFile.SolarSystem;
            _currentCaptain = saveFile.Captain;
            _shipService.Ship = saveFile.Ship;
            _eventService.GameEvents = saveFile.GameEvents;
        }
    }
}
