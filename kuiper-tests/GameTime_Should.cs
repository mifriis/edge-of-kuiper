using System;
using System.Threading;
using Kuiper.Domain;
using Kuiper.Services;
using Shouldly;
using Xunit;

namespace Kuiper.Tests
{
    public class GameTime_Should
    {
        private readonly DateTimeOffset _gameTimeGenesis = new DateTimeOffset(new DateTime(2078, 1, 8));
        private readonly long _tickAccelerationConstant = 7; //1 real day is 7 game days
        
        [Fact]
        public void ThrowException_When_TimeIsBeforeGenesis()
        {
            //Act
            //Assert
            //Arrange
            Should.Throw<ArgumentException>(() => new GameTime(_gameTimeGenesis - TimeSpan.FromDays(1)));
        }   
        
        [Fact]
        public void DilateTime_When_CalculatingElapsedGameTime()
        {
            //Arrange
            TimeService.Init(DateTimeOffset.Now);
            var past = new GameTime(_gameTimeGenesis);
            
            //Act
            Thread.Sleep(TimeSpan.FromSeconds(2));
            var future = GameTime.Now();

            //Assert
            past.Value.ShouldBeLessThan(future.Value);

            future.Value.ShouldBeGreaterThanOrEqualTo(past.Value.Add(TimeSpan.FromSeconds(2 * _tickAccelerationConstant)));
        }
    }
}
