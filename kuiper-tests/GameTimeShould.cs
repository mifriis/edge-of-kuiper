using Kuiper.Systems;
using System;
using Xunit;

namespace Kuiper.Tests.Unit.Systems
{
    public class GameTimeStaticShould
    {
        [Fact]
        public void ThrowExceptionIfRealStartTimeNeverSet()
        {
            //Arrange
            GameTime.RealStartTime = DateTime.MinValue;
            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GameTime.Now());
        }

        [Fact]
        public void ReturnGamDateInNextWeekWhenStartingRealYesterday()
        {
            //Arrange
            GameTime.RealStartTime = DateTime.Now.Subtract(TimeSpan.FromHours(24));

            //Act

            var gameNow = GameTime.Now();

            //Assert
            Assert.Equal(8, gameNow.Day);
            Assert.Equal(1, gameNow.Month);
            Assert.Equal(2078, gameNow.Year);
        }
    }
}
