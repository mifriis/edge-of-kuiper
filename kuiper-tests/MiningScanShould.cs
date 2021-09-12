using System;
using Xunit;
using Kuiper.Domain;
using NSubstitute;
using Kuiper.Services;

namespace Kuiper.Tests.Unit.Domain
{
    public class MiningScanShould
    {
        [Fact]
        public void EndEventWithFailure()
        {
            //Arrange
            var diceRoller = Substitute.For<IDiceRoller>();
            DiceFactory.setDiceRollerInstance(diceRoller);
            diceRoller.D100(default,default).ReturnsForAnyArgs(false);
            var evt = new MiningScan(DateTime.Now);
            
            //Act
            var result = evt.EndEvent();

            //Assert
            Assert.Equal("Asteroid scanning complete, but found no asteroids with a yield within the scanners parameters.", result);
        }

        [Fact]
        public void EndEventWithFSuccess()
        {
            //Arrange
            var diceRoller = Substitute.For<IDiceRoller>();
            var randomer = Substitute.For<IRandom>();
            var captain = new Captain("Testo") { Ship = new Ship("Testlo", "Testlo", 100) { CurrentLocation = Locations.Earth}};
            CaptainLocator.SetCaptain(captain);
            DiceFactory.setDiceRollerInstance(diceRoller);
            DiceFactory.setRandomInstance(randomer);
            diceRoller.D100(default,default).ReturnsForAnyArgs(true);
            randomer.Next(default,default).ReturnsForAnyArgs(100);

            var evt = new MiningScan(DateTime.Now);
            
            //Act
            var result = evt.EndEvent();

            //Assert
            Assert.Equal("Asteroid scanning complete. Found a candidate 100km from Earth. Set a course to begin mining the asteroid.", result);

        }
    }
}
