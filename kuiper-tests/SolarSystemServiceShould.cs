using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Kuiper.Tests.Unit.Services
{
    public class SolarSystemServiceShould
    {
        private List<CelestialBody> createTestData() {
            var sun = CelestialBody.Create<Star>("Sun", 0,0,0,null);
            var mercury = CelestialBody.Create<Planet>("Mercury", 0.387, 47.4, 30, sun);
            var venus = CelestialBody.Create<Planet>("Venus", 0.723, 35.4, 170, sun);
            var earth = CelestialBody.Create<Planet>("Earth", 1, 29.8, 170, sun);
            var moon = CelestialBody.Create<Moon>("Luna", 0.00257356604, 1.022, 125, earth);
            var mars = CelestialBody.Create<Planet>("Mars", 1.523, 24.1, 95, sun);
            var jupiter = CelestialBody.Create<GasGiant>("Jupiter", 5.205, 13.1, 45, sun);
            var saturn = CelestialBody.Create<Planet>("Saturn", 9.582, 9.7, 345, sun);

            var testData = new List<CelestialBody>(){
                sun, mercury, venus, earth, moon, mars, jupiter, saturn
            };

            return testData;
        }

        [Fact]
        public void ReturnBodyBasedOnName()
        {
            //Arrange
            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object);

            //Act
            var body = solarSystemService.GetBody("Saturn");

            //Assert
            Assert.Equal(testData.SingleOrDefault(b => b.Name == "Saturn"), body);
        }

        [Fact]
        public void ReturnCorrectAuDistanceBetweenBodies()
        {
            //Arrange
            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            GameTime.RealStartTime = DateTime.Now.Subtract(TimeSpan.FromHours(24));

            var solarSystemService = new SolarSystemService(repository.Object);

            //Act
            var origin = solarSystemService.GetBody("Earth");
            var destination = solarSystemService.GetBody("Mars");

            var distance = solarSystemService.GetDistanceInAu(origin, destination);

            //Assert
            Assert.Equal(1.6427351236343384, distance, 6);
        }

        [Fact]
        public void ReturnCorrectKmDistanceBetweenBodies()
        {
            //Arrange
            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            GameTime.RealStartTime = DateTime.Now.Subtract(TimeSpan.FromHours(24));

            var solarSystemService = new SolarSystemService(repository.Object);

            //Act
            var origin = solarSystemService.GetBody("Earth");
            var destination = solarSystemService.GetBody("Mars");

            var distance = solarSystemService.GetDistanceInKm(origin, destination);

            //Assert
            Assert.InRange(distance, 245749650, 245749685);
        }
    }
}