using System;
using System.Threading;
using Kuiper.Domain;
using Shouldly;
using Xunit;

namespace Kuiper.Tests
{
    [Collection("Sequential")]
    public class GameTimeShould
    {
        private readonly DateTime _gameTimeEpoch = new DateTime(2078, 1, 8);
        private readonly long _tickAccelerationConstant = 7; //1 real day is 7 game days
        
        [Fact]
        public void Return_GameTime_Epoch_For_A_Real_Seed()
        {
            //Arrange
            var realTimeStartDate = new DateTime(2020, 2, 2);
            var gametime = new GameTime(realTimeStartDate.Ticks);
            
            //Act
            var dateTimeFormat = gametime.ConvertToGameDateTime();

            //Assert
            
            dateTimeFormat.Year.ShouldBe(_gameTimeEpoch.Year);
            dateTimeFormat.Month.ShouldBe(_gameTimeEpoch.Month);
            dateTimeFormat.Day.ShouldBe(_gameTimeEpoch.Day);
        }   
        
        [Fact]
        public void Add_One_Day_When_Adding_One_Day()
        {
            //Arrange
            var gametime = new GameTime(0);
            var oneDay = TimeSpan.FromDays(1).Ticks;
            
            //Act
            var future = gametime.Add(oneDay);

            //Assert
            future.Ticks.ShouldBe(gametime.Ticks + oneDay);
        } 
        
        [Fact]
        public void Convert_To_DateTime_When_Adding_Game_Day()
        {
            //Arrange
            var gametime = new GameTime(0);
            var oneDay = TimeSpan.FromDays(1).Ticks;
            
            //Act
            var future = gametime.Add(oneDay);
            var futureDateTime = future.ConvertToGameDateTime();

            //Assert
            futureDateTime.Subtract(_gameTimeEpoch).ShouldBe(TimeSpan.FromDays(1));
        }
        
        [Fact]
        public void Two_Real_Dates_Should_Be_Dilated()
        {
            //Arrange
            var now = DateTime.UtcNow.Ticks;
            var past = new GameTime(now);
            
            //Act
            Thread.Sleep(TimeSpan.FromSeconds(2));
            var future = GameTime.Now();

            //Assert
            past.Ticks.ShouldBeLessThan(future.Ticks);
            future.Ticks.ShouldBeGreaterThanOrEqualTo(2 * TimeSpan.TicksPerSecond * _tickAccelerationConstant);
        }
    }
}
