using System;
using Xunit;
using Kuiper.Systems;

namespace Kuiper.Tests.Unit.Systems
{
    public class TimeDilationTests
    {
        [Fact]
        public void ReturnNewGameDate()
        {
            //Arrange
            var gameNow = TimeDilation.GameStartDate;
            var realLastSeen = new DateTime(2020,1,1,1,1,1,1);
            var realNow = new DateTime(2020,1,2,1,1,1,1);

            //Act

            var newGameDate = TimeDilation.CalculateTime(gameNow, realLastSeen, realNow);

            //Assert
            Assert.Equal(new DateTime(2078,1,8), newGameDate);
        }
    }
}
