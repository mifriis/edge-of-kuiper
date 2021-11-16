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

namespace Kuiper.Tests.Unit.Services
{
    public class CaptainServiceShould
    {
        [Fact]
        public void SetupANewCaptainIfNoSavesFound()
        {
            //Arrange
            var captainInput = new StringReader("LongLars");
            Console.SetIn(captainInput);

            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var shipService = new Mock<IShipService>();
            var eventService = new Mock<IEventService>();
            var saveService = new Mock<ISaveService>();

            saveService.Setup(x => x.LookForSaves("LongLars")).Returns(new List<string>());
            solarSystemService.Setup(x => x.GetBody("Earth")).Returns(currentLocation);
            var captainService = new CaptainService(solarSystemService.Object, shipService.Object, eventService.Object, saveService.Object);

            //Act
            var captain = captainService.SetupCaptain();
            
            //Assert
            Assert.True(captain.Ship != null);
            Assert.Equal(captainService.GetCaptain(), captain);
            Assert.True(GameTime.Now() != DateTime.MinValue);
            Assert.True(captain.LastLoggedIn != DateTime.MinValue);
            Assert.Equal(captain.Ship.CurrentLocation, currentLocation);
        }

        [Fact]
        public void LoadCaptainWhenSavesFound()
        {
            //Arrange
            var captainInput = new StringReader("LongLars");
            Console.SetIn(captainInput);

            var account = new Account(0);
            var ship = new Ship("Fleggaard", "Mafia", 123);
            var captain = new Captain("LongLars",account) { Ship = ship, StartTime = new DateTime(2070,2,3) };

            var save = new SaveFile() { Ship = ship, Captain = captain};

            var solarSystemService = new Mock<ISolarSystemService>();
            var shipService = new Mock<IShipService>();
            var eventService = new Mock<IEventService>();
            var saveService = new Mock<ISaveService>();

            saveService.Setup(x => x.LookForSaves("LongLars")).Returns(new List<string>() { "LongLars" });
            saveService.Setup(x => x.Load("LongLars")).Returns(save);
            var captainService = new CaptainService(solarSystemService.Object, shipService.Object, eventService.Object, saveService.Object);

            //Act
            var currentCaptain = captainService.SetupCaptain();
            
            //Assert
            Assert.Equal(captain.Ship, ship);
            Assert.Equal(captainService.GetCaptain(), captain);
            Assert.True(GameTime.Now() != DateTime.MinValue);
            Assert.Equal(captain.StartTime, new DateTime(2070,2,3));
        }
    }
}