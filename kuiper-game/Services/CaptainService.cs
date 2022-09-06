using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Domain.Ship;
using Kuiper.Systems;

namespace Kuiper.Services
{
    public class CaptainService : ICaptainService
    {
        private readonly ISolarSystemService _solarSystemService;
        private readonly IEventService _eventService;
        private readonly IShipService _shipService;
        private readonly ISaveService _saveService;
        private readonly IGameTimeService _gameTimeService;
        private readonly IAccountService _accountService;

        public CaptainService(ISolarSystemService solarSystemService, IShipService shipService, IEventService eventService, ISaveService saveService, IGameTimeService gameTimeService, IAccountService accountService)
        {
            _solarSystemService = solarSystemService;
            _eventService = eventService;
            _shipService = shipService;
            _saveService = saveService;
            _gameTimeService = gameTimeService;
            _accountService = accountService;
        }
        private Captain _currentCaptain;
        

        //This whole class is a bit of a mess of "load" and testing data from some systems.
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
                _currentCaptain = new Captain(name, DateTime.Now, new Account(100M));
                _gameTimeService.RealStartTime = _currentCaptain.StartTime;
                _accountService.Account = _currentCaptain.Account;
                
                _accountService.Deposit(239048M);
                _accountService.Deposit(23M);
                _accountService.Withdraw(123M);

                _solarSystemService.LoadFromRepository();
                
                _currentCaptain.LastLoggedIn = _gameTimeService.Now();
                _currentCaptain.Ship = new Ship("Bullrun", new ShipEngine(10000,3,1000000,1100000), 250) { FuelMass = 100 };
                
                _currentCaptain.Ship.CurrentLocation = _solarSystemService.GetBody("Earth");
                ConsoleWriter.Write($"Welcome, Captain {name}, you have logged in on {_gameTimeService.Now()}");
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
            saveFile.Asteroids = _solarSystemService.Asteroids;
            saveFile.GameEvents = _eventService.GameEvents;                        
            saveFile.Ship = _shipService.Ship;
            _currentCaptain.LastLoggedIn = _gameTimeService.Now();
            saveFile.Captain = _currentCaptain;
            _saveService.Save(saveFile);
        }

        public void LoadGame(string save)
        {
            var saveFile = _saveService.Load(save);
            _solarSystemService.SolarSystem = saveFile.SolarSystem;
            _solarSystemService.Asteroids = saveFile.Asteroids;
            _currentCaptain = saveFile.Captain;
            _shipService.Ship = saveFile.Ship;
            _eventService.GameEvents = saveFile.GameEvents;
            _gameTimeService.RealStartTime = _currentCaptain.StartTime;
            _eventService.ExecuteEvents(_gameTimeService.Now());
        }
    }
}
