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
    }
}
