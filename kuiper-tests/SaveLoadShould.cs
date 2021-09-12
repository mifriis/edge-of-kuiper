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
            captain.Ship.CurrentLocation.Sattelites.Add(new Location("Asteroid",20000,new System.Collections.Generic.List<Location>(), SatteliteType.Asteroid));
            
            system.Captain = captain;
            //Act
            SaveLoad.SaveGame(system);
            var output = SaveLoad.Load($"{captName}.save");
            //Assert
            Assert.Equal(output.GameStart, system.GameStart);
            Assert.Equal(SatteliteType.Planet, output.Captain.Ship.CurrentLocation.SatteliteType);
            Assert.Equal(SatteliteType.Moon, output.Captain.Ship.TargetLocation.SatteliteType);
            Assert.Equal(SatteliteType.Moon, output.Captain.Ship.CurrentLocation.Sattelites[0].SatteliteType);
            Assert.Equal(SatteliteType.Asteroid, output.Captain.Ship.CurrentLocation.Sattelites[1].SatteliteType);
        }
    }
}
