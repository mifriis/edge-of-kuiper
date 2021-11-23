using Kuiper.Services;
using Kuiper.Systems;
using System;
using Xunit;

namespace Kuiper.Tests.Unit.Systems
{
    public class GameTimeServiceShould
    {
        [Fact]
        public void ThrowExceptionIfRealStartTimeNeverSet()
        {
            //Arrange
            var service = new GameTimeService();
            service.RealStartTime = DateTime.MinValue;
            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => service.Now());
        }

        [Fact]
        public void ReturnGamDateInNextWeekWhenStartingRealYesterday()
        {
            //Arrange
            var service = new GameTimeService();
            service.RealStartTime = DateTime.Now.Subtract(TimeSpan.FromHours(24));
            //Act

            var gameNow = service.Now();

            //Assert
            Assert.Equal(8, gameNow.Day);
            Assert.Equal(1, gameNow.Month);
            Assert.Equal(2078, gameNow.Year);
        }

        [Fact]
        public void ReturnGameStartDate()
        {
            //Arrange
            var service = new GameTimeService();
            
            //Act
            var startDate = service.GameStartDate;

            //Assert
            Assert.Equal(new DateTime(2078, 1, 1), startDate);

        }
    }
}
