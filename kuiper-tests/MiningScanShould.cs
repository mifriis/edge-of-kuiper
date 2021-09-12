using System;
using Xunit;
using Kuiper.Domain;
using NSubstitute;
using Kuiper.Services;

namespace Kuiper.Tests.Unit.Domain
{
    [Collection("Sequential")]
    public class MiningScanShould
    {
        [Fact]
        public void EndEventWithFailure()
        {
            //Arrange
            var diceRoller = Substitute.For<IDiceRoller>();
            DiceFactory.setDiceRollerInstance(null);
            DiceFactory.setDiceRollerInstance(diceRoller);
            diceRoller.D100(default,default).ReturnsForAnyArgs(false);
            var evt = new MiningScan(DateTime.Now);
            
            //Act
            var result = evt.EndEvent();

            //Assert
            diceRoller.ReceivedWithAnyArgs(1).D100(default,default);
            Assert.Equal("Asteroid scanning complete, but found no asteroids with a yield within the scanners parameters.", result);
        }

        [Fact]
        public void EndEventWithSuccess()
        {
            //Arrange
            DiceFactory.setDiceRollerInstance(null);
            var diceRoller = Substitute.For<IDiceRoller>();
            var randomer = Substitute.For<IRandom>();
            var gamestart = new DateTime(1990,1,1,1,1,1);
            var system = new SolarSystem(gamestart);
            var captain = new Captain("Testo") { Ship = new Ship("Testlo", "Testlo", 100) { CurrentLocation = Locations.Earth}};
            system.Captain = captain;
            SolarSystemLocator.SetSolarSystem(system);
            DiceFactory.setDiceRollerInstance(diceRoller);
            DiceFactory.setRandomInstance(randomer);
            diceRoller.D100(default,default).ReturnsForAnyArgs(true);
            randomer.Next(default,default).ReturnsForAnyArgs(100);

            var evt = new MiningScan(DateTime.Now);
            
            //Act
            var result = evt.EndEvent();

            //Assert
            diceRoller.ReceivedWithAnyArgs(1).D100(default,default);
            Assert.Equal("Asteroid scanning complete. Found a candidate 100km from Earth. Set a course to begin mining the asteroid.", result);

        }
    }
}
