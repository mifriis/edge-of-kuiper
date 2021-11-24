using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;
using Kuiper.Domain;
using System.IO;
using Kuiper.Systems.Events;
using Kuiper.Domain.Ship;

namespace Kuiper.Tests.Unit.Services
{
    public class CaptainServiceShould
    {
        private List<CelestialBody> createTestData() {
            var sun = CelestialBody.Create("Sun", 0,0,0,null,CelestialBodyType.Star);
            var mercury = CelestialBody.Create("Mercury", 0.387, 47.4, 30, sun, CelestialBodyType.Planet);
            var venus = CelestialBody.Create("Venus", 0.723, 35.4, 170, sun, CelestialBodyType.Planet);
            var earth = CelestialBody.Create("Earth", 1, 29.8, 170, sun, CelestialBodyType.Planet);
            var moon = CelestialBody.Create("Luna", 0.00257356604, 1.022, 125, earth, CelestialBodyType.Moon);
            var mars = CelestialBody.Create("Mars", 1.523, 24.1, 95, sun, CelestialBodyType.Planet);
            var jupiter = CelestialBody.Create("Jupiter", 5.205, 13.1, 45, sun, CelestialBodyType.GasGiant);
            var saturn = CelestialBody.Create("Saturn", 9.582, 9.7, 345, sun, CelestialBodyType.Planet);

            var testData = new List<CelestialBody>(){
                sun, mercury, venus, earth, moon, mars, jupiter, saturn
            };

            return testData;
        }

        [Fact]
        public void ReturnCaptainIfAlreadySetup()
        {
            //Arrange
            Console.Clear();
             var captainInput = new StringReader("LongLars");
            Console.SetIn(captainInput);

            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var shipService = new Mock<IShipService>();
            var eventService = new Mock<IEventService>();
            var saveService = new Mock<ISaveService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var accountService = new Mock<IAccountService>();

            gameTimeService.Setup(u => u.Now()).Returns(DateTime.Now);
            saveService.Setup(x => x.LookForSaves("LongLars")).Returns(new List<string>());
            solarSystemService.Setup(x => x.GetBody("Earth")).Returns(currentLocation);
            var captainService = new CaptainService(solarSystemService.Object, shipService.Object, eventService.Object, saveService.Object, gameTimeService.Object, accountService.Object);
            var firstCaptain = captainService.SetupCaptain();
            //Act
            var captain = captainService.SetupCaptain();
            
            //Assert
            saveService.Verify(x => x.LookForSaves(It.IsAny<string>()), Times.Exactly(1));
            solarSystemService.Verify(x => x.GetBody(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void ThrowIfCaptainIsNotSet()
        {
            //Arrange
            Console.Clear();
            var solarSystemService = new Mock<ISolarSystemService>();
            var shipService = new Mock<IShipService>();
            var eventService = new Mock<IEventService>();
            var saveService = new Mock<ISaveService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var accountService = new Mock<IAccountService>();

            var captainService = new CaptainService(solarSystemService.Object, shipService.Object, eventService.Object, saveService.Object, gameTimeService.Object, accountService.Object);
            //Act
            //Assert
            Assert.Throws<NullReferenceException>(() => captainService.GetCaptain());
        }

        [Fact]
        public void SaveSuccessfully()
        {
            //Arrange
            Console.Clear();
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var captainInput = new StringReader("LongLars");
            Console.SetIn(captainInput);
            var events = new List<IEvent>();
            var solarSystemService = new Mock<ISolarSystemService>();
            var ship = new Ship("Slippy",new ShipEngine(10000,3,1000000,1100000), 250);
            
            var shipService = new Mock<IShipService>();
            var eventService = new Mock<IEventService>();
            var saveService = new Mock<ISaveService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var accountService = new Mock<IAccountService>();

            saveService.Setup(x => x.LookForSaves("LongLars")).Returns(new List<string>());
            solarSystemService.Setup(x => x.GetBody("Earth")).Returns(currentLocation);
            eventService.SetupGet(x => x.GameEvents).Returns(events);
            solarSystemService.SetupGet(x => x.SolarSystem).Returns(createTestData());
            shipService.SetupGet(x => x.Ship).Returns(ship);
            
            var captainService = new CaptainService(solarSystemService.Object, shipService.Object, eventService.Object, saveService.Object, gameTimeService.Object, accountService.Object);
            captainService.SetupCaptain();
            captainService.GetCaptain().Ship = ship;

            
            //Act
            captainService.SaveGame();
            
            //Assert
            saveService.Verify(x => x.Save(It.IsAny<SaveFile>()), Times.Exactly(1));
            solarSystemService.Verify(x => x.SolarSystem, Times.Exactly(1));
            eventService.Verify(x => x.GameEvents, Times.Exactly(1));
            shipService.Verify(x => x.Ship, Times.Exactly(1));
            
            
        }

        [Fact]
        public void SetupANewCaptainIfNoSavesFound()
        {
            //Arrange
            Console.Clear();
            var captainInput = new StringReader("LongLars");
            Console.SetIn(captainInput);

            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var shipService = new Mock<IShipService>();
            var eventService = new Mock<IEventService>();
            var saveService = new Mock<ISaveService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var accountService = new Mock<IAccountService>();

            saveService.Setup(x => x.LookForSaves("LongLars")).Returns(new List<string>());
            solarSystemService.Setup(x => x.GetBody("Earth")).Returns(currentLocation);
            gameTimeService.Setup(u => u.Now()).Returns(DateTime.Now);
            var captainService = new CaptainService(solarSystemService.Object, shipService.Object, eventService.Object, saveService.Object, gameTimeService.Object, accountService.Object);

            //Act
            var captain = captainService.SetupCaptain();
            
            //Assert
            Assert.True(captain.Ship != null);
            Assert.Equal(captainService.GetCaptain(), captain);
            Assert.True(captain.LastLoggedIn != DateTime.MinValue);
            Assert.Equal(captain.Ship.CurrentLocation, currentLocation);
            gameTimeService.VerifySet(async x => x.RealStartTime=It.IsAny<DateTime>(), Times.Exactly(1));
        }

        [Fact]
        public void LoadCaptainWhenSavesFound()
        {
            //Arrange
            Console.Clear();
            var captainInput = new StringReader("LongLars");
            Console.SetIn(captainInput);

            var account = new Account(0);
            var ship = new Ship("Fleggaard", new ShipEngine(10000,3,1000000,1100000), 250);
            var captain = new Captain("LongLars",account) { Ship = ship, StartTime = new DateTime(2070,2,3) };

            var save = new SaveFile() { Ship = ship, Captain = captain};

            var solarSystemService = new Mock<ISolarSystemService>();
            var shipService = new Mock<IShipService>();
            var eventService = new Mock<IEventService>();
            var saveService = new Mock<ISaveService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var accountService = new Mock<IAccountService>();

            saveService.Setup(x => x.LookForSaves("LongLars")).Returns(new List<string>() { "LongLars" });
            saveService.Setup(x => x.Load("LongLars")).Returns(save);
            var captainService = new CaptainService(solarSystemService.Object, shipService.Object, eventService.Object, saveService.Object, gameTimeService.Object, accountService.Object);

            //Act
            var currentCaptain = captainService.SetupCaptain();
            
            //Assert
            Assert.Equal(captain.Ship, ship);
            Assert.Equal(captainService.GetCaptain(), captain);
            gameTimeService.VerifySet(async x => x.RealStartTime=It.IsAny<DateTime>(), Times.Exactly(1));
            Assert.Equal(captain.StartTime, new DateTime(2070,2,3));
        }
    }
}