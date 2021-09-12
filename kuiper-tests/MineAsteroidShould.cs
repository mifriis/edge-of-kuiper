using System;
using Xunit;
using Kuiper.Domain;
using NSubstitute;
using Kuiper.Services;

namespace Kuiper.Tests.Unit.Domain
{
    [Collection("Sequential")]
    public class MineAsteroidShould
    {
        [Fact]
        public void EndEventWithFailure()
        {
            //Arrange
            var diceRoller = Substitute.For<IDiceRoller>();
            DiceFactory.setDiceRollerInstance(null);
            DiceFactory.setRandomInstance(null);
            DiceFactory.setDiceRollerInstance(diceRoller);
            diceRoller.D100(default, default).ReturnsForAnyArgs(false);
            var evt = new MineAsteroid(DateTime.Now, TimeSpan.FromHours(7));
            
            //Act
            var result = evt.EndEvent();

            //Assert
            diceRoller.ReceivedWithAnyArgs(8).D100(default,default);
            Assert.Equal("After mining for 7 hours, you have filled your cargohold with 0 tons of minerals", result);
        }

        [Fact]
        public void EndEventWithSuccess()
        {
             //Arrange
            var diceRoller = Substitute.For<IDiceRoller>();
            DiceFactory.setDiceRollerInstance(null);
            DiceFactory.setDiceRollerInstance(diceRoller);
            diceRoller.D100(default,default).ReturnsForAnyArgs(true);
            var evt = new MineAsteroid(DateTime.Now, TimeSpan.FromHours(7));
            
            //Act
            var result = evt.EndEvent();

            //Assert
            diceRoller.ReceivedWithAnyArgs(8).D100(default,default);
            Assert.Equal("After mining for 7 hours, you have filled your cargohold with 8 tons of minerals", result);

        }
    }
}
