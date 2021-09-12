using System;
using Xunit;
using Kuiper.Domain;
using NSubstitute;
using Kuiper.Services;
using Kuiper.Systems;

namespace Kuiper.Tests.Integration.Systems
{
    public class SaveLoadShould
    {
        [Fact]
        public void SaveAndLoadSuccessfully()
        {
            //Arrange
            var startDate = new DateTime(1990,1,1,1,1,1);
            var captName = Guid.NewGuid();
            var captain = new Captain(captName.ToString());
            var system = new SolarSystem(startDate);
            system.Captain = captain;

            //Act
            SaveLoad.SaveGame(system);
            var output = SaveLoad.Load($"{captName}.save");
            //Assert
            Assert.Equal(system.GameStart, output.GameStart);
        }

        [Fact]
        public void SaveAndLoadSuccessfullyWithLocations()
        {
            //Arrange
            var startDate = new DateTime(1990,1,1,1,1,1);
            var system = new SolarSystem(startDate);
            system.Locations.Add(Locations.Earth);
            system.Locations.Add(Locations.Luna);
            var captName = Guid.NewGuid();
            var captain = new Captain(captName.ToString());
            captain.Ship = new Ship("test","test", 10);
            captain.Ship.CurrentLocation = Locations.Earth;
            captain.Ship.TargetLocation = Locations.Luna;
            captain.Ship.CurrentLocation.Satellites.Add(new Location("Asteroid",20000,new System.Collections.Generic.List<Location>(), SatelliteType.Asteroid));
            
            system.Captain = captain;
            //Act
            SaveLoad.SaveGame(system);
            var output = SaveLoad.Load($"{captName}.save");
            //Assert
            Assert.Equal(output.GameStart, system.GameStart);
            Assert.Equal(SatelliteType.Planet, output.Captain.Ship.CurrentLocation.SatelliteType);
            Assert.Equal(SatelliteType.Moon, output.Captain.Ship.TargetLocation.SatelliteType);
            Assert.Equal(SatelliteType.Moon, output.Captain.Ship.CurrentLocation.Satellites[0].SatelliteType);
            Assert.Equal(SatelliteType.Asteroid, output.Captain.Ship.CurrentLocation.Satellites[1].SatelliteType);
        }
    }
}
