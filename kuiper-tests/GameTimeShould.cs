using Kuiper.Systems;
using System;
using Xunit;

namespace Kuiper.Tests.Unit.Systems
{
    [Collection("Sequential")]
    public class GameTimeStaticShould
    {
        [Fact]
        public void ThrowExceptionIfRealStartTimeNeverSet()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GameTime.Now());
        }
    }

    [Collection("Sequential")]
    public class GameTimeShould
    {
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
