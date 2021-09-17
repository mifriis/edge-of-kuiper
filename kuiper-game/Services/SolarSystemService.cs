using System;
using System.Linq;
using Kuiper.Domain;
using Kuiper.Systems;
using System.Collections.Generic;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System.Numerics;

namespace Kuiper.Services
{
    public class SolarSystemService : ISolarSystemService
    {
        private SolarSystem _currentSystem;
        private List<CelestialBody> SolarSystem;
        public SolarSystemService(ISolarSystemRepository repository)
        {
            SolarSystem = new List<CelestialBody>();
            
            if(repository != null)
                SolarSystem = CreateSolarSystem(repository).ToList();
        }

        private IEnumerable<CelestialBody> CreateSolarSystem(ISolarSystemRepository repository) {
            
            return repository.GetSolarSystem();
        }
        public SolarSystem SetupGame()
        {
            if(_currentSystem == null) 
            {
                ConsoleWriter.Write("Greetings captain, what is your name?");
                var name = Console.ReadLine();
                var saves = SaveLoad.LookForSaves(name);
                if(saves.Count() > 0)
                {
                    //We are now loading
                    _currentSystem = SaveLoad.Load(saves.FirstOrDefault());
                    GameTime.RealStartTime = _currentSystem.GameStart;
                    SolarSystemLocator.SetSolarSystem(_currentSystem);
                    SolarSystemLocator.SolarSystem.SolarSystemService = this;
                    ConsoleWriter.Write($"Welcome back Captain {_currentSystem.Captain.Name}, you were last seen on {_currentSystem.Captain.LastLoggedIn}!");    
                    _currentSystem.Captain.Ship.StatusReport();
                    return _currentSystem;
                }
                //We are now starting a new game
                _currentSystem = new SolarSystem(DateTime.Now);
                _currentSystem.Captain = new Captain(name);
                GameTime.RealStartTime = _currentSystem.GameStart;
                SolarSystemLocator.SetSolarSystem(_currentSystem);
                SolarSystemLocator.SolarSystem.SolarSystemService = this;
                SolarSystemLocator.SolarSystem.CelestialBodies = SolarSystemLocator.SolarSystem.SolarSystemService.GetBodies().ToList();
                _currentSystem.Captain.Ship = new Ship("Bullrun","Sloop", 40000);
                _currentSystem.Captain.Ship.CurrentLocation = GetBody("Earth");
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
        private const int AUINKM =  149597871; // 1 AU in KM. Maybe this belongs somewhere else?

        public IEnumerable<CelestialBody> GetBodies(CelestialBodyType type)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CelestialBody> GetBodies()
        {
            return SolarSystem;
        }

        public CelestialBody GetBody(string name)
        {
            return SolarSystem.Where(b => string.Equals(name, b.Name, System.StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }

        public double GetDistanceInAu(CelestialBody origin, CelestialBody destination)
        {

            var originPosition = origin.GetPosition(GameTime.ElapsedGameTime);
            var destinationPosition = destination.GetPosition(GameTime.ElapsedGameTime);

            return Vector2.Distance(originPosition, destinationPosition);
        }

        public long GetDistanceInKm(CelestialBody origin, CelestialBody destination)
        {
            return Convert.ToInt64(GetDistanceInAu(origin, destination) * AUINKM);
        }

        public IEnumerable<CelestialBody> GetNearestBodies(int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CelestialBody> GetSatellites(CelestialBody parent)
        {
            throw new System.NotImplementedException();
        }
    }
}
