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
            var sun = CelestialBody.Create("Sun", 0,0,0,null,CelestialBodyType.Star);
            var mercury = CelestialBody.Create("Mercury", 0.387, 47.4, 30, sun, CelestialBodyType.Planet);
            var venus = CelestialBody.Create("Venus", 0.723, 35.4, 170, sun, CelestialBodyType.Planet);
            var earth = CelestialBody.Create("Earth", 1, 29.8, 170, sun, CelestialBodyType.Planet);
            var moon = CelestialBody.Create("Luna", 0.00257356604, 1.022, 125, earth, CelestialBodyType.Moon);
            var mars = CelestialBody.Create("Mars", 1.523, 24.1, 95, sun, CelestialBodyType.Planet);
            var jupiter = CelestialBody.Create("Jupiter", 5.205, 13.1, 45, sun, CelestialBodyType.GasGiant);
            var saturn = CelestialBody.Create("Saturn", 9.582, 9.7, 345, sun, CelestialBodyType.Planet);

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
            solarSystemService.LoadFromRepository();

            //Act
            var body = solarSystemService.GetBody("Saturn");

            //Assert
            Assert.Equal(testData.SingleOrDefault(b => b.Name == "Saturn"), body);
        }

        [Fact]
        public void ReturnAllBodiesUnderAStar()
        {
            //Arrange
            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object);
            solarSystemService.LoadFromRepository();

            //Act
            var bodies = solarSystemService.GetBodies();

            //Assert
            Assert.Equal(bodies.Count(), 6);
            Assert.True(bodies.FirstOrDefault(x => x.CelestialBodyType == CelestialBodyType.Star) == null);
            Assert.True(bodies.FirstOrDefault(x => x.CelestialBodyType == CelestialBodyType.Moon) == null);
        }

        [Fact]
        public void ReturnSattelitesAroundABody()
        {
            //Arrange
            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object);
            solarSystemService.LoadFromRepository();

            var earth = testData.FirstOrDefault(x => x.Name == "Earth");

            //Act
            var sats = solarSystemService.GetSatellites(earth);

            //Assert
            Assert.Equal(sats.Count(), 1);
            Assert.Equal(sats.Select(x => x.CelestialBodyType == CelestialBodyType.Star).Count(),1);
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
            solarSystemService.LoadFromRepository();

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
            solarSystemService.LoadFromRepository();

            //Act
            var origin = solarSystemService.GetBody("Earth");
            var destination = solarSystemService.GetBody("Mars");

            var distance = solarSystemService.GetDistanceInKm(origin, destination);

            //Assert
            Assert.InRange(distance, 245749650, 245749685);
        }
    }
}