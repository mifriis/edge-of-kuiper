using System;
using Kuiper.Services;
using NSubstitute;
using Xunit;

namespace Kuiper.Tests.Unit.Systems
{
    public class DiceRollerShould
    {
        [Fact]
        public void ReturnTrueWhenTargetIsLowerThanSeed()
        {
            //Arrange
            var random = Substitute.For<IRandom>();
            var service = new DiceRoller(random);
            random.Next(1,100).Returns(50);

            //Act
            var result = service.D100(90,0);
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ReturnFalseWhenTargetIsLowerThanSeed()
        {
            //Arrange
            var random = Substitute.For<IRandom>();
            var service = new DiceRoller(random);
            random.Next(1,100).Returns(50);

            //Act
            var result = service.D100(49,0);
            
            //Assert
            Assert.False(result);
        }

        [Fact]
        public void ReturnTrueWhenTargetPlusModifierIsLowerThanSeed()
        {
            //Arrange
            var random = Substitute.For<IRandom>();
            var service = new DiceRoller(random);
            random.Next(1,100).Returns(50);

            //Act
            var result = service.D100(48,3);
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ReturnTrueWhenD6TargetIsLowerThanSeed()
        {
            //Arrange
            var random = Substitute.For<IRandom>();
            var service = new DiceRoller(random);
            random.Next(1,6).Returns(3);

            //Act
            var result = service.D6(4,0);
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ReturnTrueWhenD12TargetIsLowerThanSeed()
        {
            //Arrange
            var random = Substitute.For<IRandom>();
            var service = new DiceRoller(random);
            random.Next(1,12).Returns(6);

            //Act
            var result = service.D12(8,0);
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ReturnTrueWhenD20TargetIsLowerThanSeed()
        {
            //Arrange
            var random = Substitute.For<IRandom>();
            var service = new DiceRoller(random);
            random.Next(1,20).Returns(10);

            //Act
            var result = service.D20(12,0);
            
            //Assert
            Assert.True(result);
        }
    }
}
